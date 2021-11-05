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
    public class Waiters
    {
        private readonly IWebAutomation auto;
        private readonly bool ensureSingleResult;

        public Waiters(IWebAutomation auto, bool ensureSingleResult = true)
        {
            this.auto = auto;
            this.ensureSingleResult = ensureSingleResult;
        }

        public ResultCollection<IWebElement> ForLoad(ByChain byChain, int waitSecond = 30)
        {
            return ForWebElements(byChain, null, null, waitSecond);
        }

        public ResultCollection<IWebElement> ForLoadWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, int waitSecond = 30, string matcherText = null)
        {
            return ForWebElements(byChain, matcher, null, waitSecond, matcherText);
        }

        public ResultCollection<IWebElement> ForAction(ByChain byChain, Action<IWebElement> action, int waitSecond = 30, string actionText = null)
        {
            return ForWebElements(byChain, null, action, waitSecond, null, actionText);
        }

        public ResultCollection<IWebElement> ForActionWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, Action<IWebElement> action, int waitSecond = 30, string matcherText = null, string actionText = null)
        {
            return ForWebElements(byChain, matcher, action, waitSecond, matcherText, actionText);
        }

        public ResultCollection<TProjection> ForProjection<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int waitSecond = 30, string projectionText = null)
        {
            return For(byChain, null, null, projection, waitSecond, null, null, projectionText);
        }
        
        public ResultCollection<TProjection> ForProjectionWithCondition<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher, Func<IWebElement, TProjection> projection, int waitSecond = 30, string matcherText = null, string projectionText = null)
        {
            return For(byChain, matcher, null, projection, waitSecond, null, matcherText, projectionText);
        }

        //ForElement, ForProjection, ForAction

        //ToDo: Script Helper also uses this -> Action == ScriptExecute

        protected ResultCollection<IWebElement> ForWebElements(ByChain byChain, Func<IWebElement, bool> matcher, Action<IWebElement> action, int waitSecond = 30, string matcherText = null, string actionText = null)
        {
            return For(byChain, matcher, action, e => e, waitSecond, matcherText, actionText, "WebElement");
        }

        protected ResultCollection<TProjection> For<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher, Action<IWebElement> action, Func<IWebElement, TProjection> projection, int waitSecond = 30, string matcherText = null, string actionText = null, string projectionText = null)
        {
            int timeout = waitSecond * 1000;

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
                    ResultCollection<TProjection> resultCollection = TryProject(webElements, projection, projectionText);
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

        private ResultCollection<TProjection> TryProject<TProjection>(List<IWebElement> webElements, Func<IWebElement, TProjection> projection, string projectionText)
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
        
        public ResultCollection<IWebElement> ForVisible(ByChain chain, int waitSecond = 30) => ForLoadWithCondition(chain, e => e.Displayed, waitSecond, "IsDisplayed");
        public ResultCollection<IWebElement> ForInvisible(ByChain chain, int waitSecond = 30) => ForLoadWithCondition(chain, e => !e.Displayed, waitSecond, "IsNotDisplayed");
        public ResultCollection<IWebElement> ForClickable(ByChain chain, int waitSecond = 30) => ForLoadWithCondition(chain, e => e.Displayed && e.Enabled, waitSecond, "Clickable");
        public ResultCollection<IWebElement> ForAttribute(ByChain chain, string attribute, int waitSecond = 30) => ForLoadWithCondition(chain, e => !string.IsNullOrEmpty(e.GetAttribute(attribute)), waitSecond, $"HasAttribute('{attribute}')");
        public ResultCollection<IWebElement> ForCssClassRemoved(ByChain chain, string className, int waitSecond = 30) => ForLoadWithCondition(chain, e => !e.GetAttribute("class").Contains(className), waitSecond, $"ClassRemoved('{className}')");
        public ResultCollection<IWebElement> ForCssClass(ByChain chain, string className, int waitSecond = 30) => ForLoadWithCondition(chain, e => e.GetAttribute("class").Contains(className), waitSecond, $"ClassAdded('{className}')");
        public ResultCollection<IWebElement> ForNotEmptyText(ByChain chain, int waitSecond = 30) => ForLoadWithCondition(chain, e => !string.IsNullOrEmpty(e.Text), waitSecond, "NotEmptyText");
    }
}