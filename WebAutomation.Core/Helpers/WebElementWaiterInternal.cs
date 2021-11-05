using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using WebAutomation.Core.Diagnostic;
using WebAutomation.Core.Exceptions;

namespace WebAutomation.Core.Helpers
{
    public class WebElementWaiterInternal
    {
        private readonly IWebAutomation auto;

        public WebElementWaiterInternal(IWebAutomation auto)
        {
            this.auto = auto;
        }

        public ResultCollection<IWebElement> ForWebElements(ByChain byChain, Func<IWebElement, bool> matcher = null, Action<IWebElement> action = null, int? waitSecond = null, string matcherText = null, string actionText = null, bool ensureSingleResult = true)
        {
            return For(byChain, matcher, action, e => e, waitSecond, matcherText, actionText, "WebElement", ensureSingleResult);
        }

        public ResultCollection<TProjection> For<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher = null, Action<IWebElement> action = null, Func<IWebElement, TProjection> projection = null, int? waitSecond = null, string matcherText = null, string actionText = null, string projectionText = null, bool ensureSingleResult = true)
        {
            int timeout = waitSecond ?? auto.Configuration.DefaultWaitTimeoutSeconds * 1000;

            var sw = Stopwatch.StartNew();

            bool found = false;
            bool matches = false;
            bool actionSuccessful = false;
            bool successfullyProjected = false;

            int retry = 1;

            ResultCollection<TProjection> projectionResult = null;
            do
            {
                var webElements = auto.Find.Elements(byChain);
                found = webElements?.Any() != null;

                if (found)
                    matches = TryMatch(webElements, matcher, matcherText);

                if (found && matches)
                    actionSuccessful = TryDoAction(webElements, action, actionText);

                if (found && matches && actionSuccessful)
                {
                    ResultCollection<TProjection> resultCollection = TryProject(webElements, projection, projectionText, ensureSingleResult);
                    successfullyProjected = resultCollection?.Any() != null;
                    projectionResult = resultCollection;
                }

                if (!successfullyProjected && sw.ElapsedMilliseconds < timeout)
                {
                    Thread.Sleep(500);

                    if (!found)
                    {
                        auto.Context.LogTrace($"Retry {retry} because Selector [{byChain}] NotFound in {sw.ElapsedMilliseconds} ms");
                    }
                    else if (!matches)
                    {
                        auto.Context.LogTrace($"Retry {retry} because Selector [{byChain}] Found BUT >> Matcher [{matcherText ?? matcher?.ToString()}] NotMatch in {sw.ElapsedMilliseconds} ms");
                    }
                    else if (!actionSuccessful)
                    {
                        auto.Context.LogTrace($"Retry {retry} because Selector [{byChain}] Found and Matcher [{matcherText ?? matcher?.ToString()}] Match BUT >> Action [{actionText ?? action?.ToString()}] NotSuccessful in {sw.ElapsedMilliseconds} ms");
                    }
                    else
                    {
                        auto.Context.LogTrace($"Retry {retry} because Selector [{byChain}] Found and Matcher [{matcherText ?? matcher?.ToString()}] Match and Action [{actionText ?? action?.ToString()}] Successful BUT >> Projection [{projectionText ?? projection?.ToString()}] NotSuccessful in {sw.ElapsedMilliseconds} ms");
                    }

                    retry++;
                }
            } while (!successfullyProjected && sw.ElapsedMilliseconds < timeout);

            if (!successfullyProjected)
            {
                if (waitSecond >= 0)
                {
                    string errorMessage;
                    if (!found)
                    {
                        errorMessage = $"Selector [{byChain}] NotFound in {sw.ElapsedMilliseconds} ms";
                    }
                    else if (!matches)
                    {
                        errorMessage = $"Selector [{byChain}] Found BUT >> Matcher [{matcherText ?? matcher?.ToString()}] NotMatch in {sw.ElapsedMilliseconds} ms";
                    }
                    else if (!actionSuccessful)
                    {
                        errorMessage = $"Selector [{byChain}] Found and Matcher [{matcherText ?? matcher?.ToString()}] Match BUT >> Action [{actionText ?? action?.ToString()}] NotSuccessful in {sw.ElapsedMilliseconds} ms";
                    }
                    else
                    {
                        errorMessage = $"Selector [{byChain}] Found and Matcher [{matcherText ?? matcher?.ToString()}] Match and Action [{actionText ?? action?.ToString()}] Successful BUT >> Projection [{projectionText ?? projection?.ToString()}] NotSuccessful in {sw.ElapsedMilliseconds} ms";
                    }

                    auto.Context.LogError(errorMessage);
                    throw new WebAutomationTimeOutException(errorMessage);
                }
            }
            else
            {
                var resultMessage = $"Selector [{byChain}] Found";
                if (matcher != null)
                    resultMessage += $" and Matcher [{matcherText ?? matcher?.ToString()}] Match";

                if (action != null)
                    resultMessage += $" and Action [{actionText ?? action?.ToString()}] Successful";

                if (projection != null)
                    resultMessage += $" and Projection [{projectionText ?? projection?.ToString()}] Successful with Projection result: ({string.Join(", ", projectionResult)})";

                resultMessage += $" in {sw.ElapsedMilliseconds} ms";

                auto.Context.LogDebug(resultMessage);
            }

            return projectionResult;
        }

        private bool TryMatch(List<IWebElement> webElements, Func<IWebElement, bool> match, string matcherText)
        {
            if (match == null)
                return true;

            try
            {
                return webElements.Any() && webElements.All(match);
            }
            catch (Exception ex)
            {
                var message = ex.Message;

                //NOTE: If profile is encoded in the message as base64, we have to cut it out to avoid explosion of the logs.
                if (message.Length > 3000)
                    message = Regex.Replace(ex.Message, "profile=.*?={1,2}", "profile=too_big_base64_string_trimmed");

                if (message.Length > 3000)
                    message = message.Substring(0, 2990) + "...trimmed";

                auto.Context.LogTrace($"Matcher [{matcherText ?? match.ToString()}] NotMatch (Reason: '{message}')");
                return false;
            }
        }

        private bool TryDoAction(List<IWebElement> webElements, Action<IWebElement> action, string actionText)
        {
            if (action == null)
                return true;

            try
            {
                if (webElements.Any())
                    webElements.ForEach(action);
                else
                    return false;
            }
            catch (Exception ex)
            {
                auto.Context.LogTrace($"Action [{actionText ?? action.ToString()}] NotSuccessful (Reason: '{ex.Message}')");
                return false;
            }

            return true;
        }

        private ResultCollection<TProjection> TryProject<TProjection>(List<IWebElement> webElements, Func<IWebElement, TProjection> projection, string projectionText, bool ensureSingleResult)
        {
            if (projection == null)
                throw new WebAutomationException("Projection can not be null!");

            try
            {
                var resultCollection = new ResultCollection<TProjection>(webElements.Select(projection).ToList());
                if (ensureSingleResult)
                    resultCollection.VerifySingleResultCount();

                return resultCollection;
            }
            catch (Exception ex)
            {
                auto.Context.LogTrace($"Projection [{projectionText ?? projection.ToString()}] NotSuccessful (Reason: '{ex.Message}')");
                return null;
            }
        }
    }
}