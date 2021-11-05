using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using WebAutomation.Core.Exceptions;

namespace WebAutomation.Core
{
    public class GetHelper
    {
        private readonly BrowserAutomation auto;

        public GetHelper(BrowserAutomation auto)
        {
            this.auto = auto;
        }

        public string Text(ByChain chain, int waitSecond = 30)
        {
            return Projection(chain, e => string.Equals(e.TagName, "input", StringComparison.InvariantCultureIgnoreCase) ? e.GetAttribute("value") : e.Text, waitSecond, "Text");
        }

        public List<string> Texts(ByChain chain, int waitSecond = 30)
        {
            return Projections(chain, e => e.Text, waitSecond, "Text");
        }

        public string Attribute(ByChain chain, string attributeName, int waitSecond = 30)
        {
            return Projection(chain, e => e.GetAttribute(attributeName), waitSecond, $"Attribute({attributeName})");
        }

        public string CssValue(ByChain chain, string cssPropertyName, int waitSecond = 30)
        {
            return Projection(chain, e => e.GetCssValue(cssPropertyName), waitSecond, $"CssValue({cssPropertyName})");
        }

        public List<string> Attributes(ByChain chain, string attributeName, int waitSecond = 30)
        {
            return Projections(chain, e => e.GetAttribute(attributeName), waitSecond, $"Attribute({attributeName})");
        }

        public int Count(ByChain chain, int waitSecond = 30)
        {
            return Projections(chain, e => 1, waitSecond, "Count").Sum();
        }

        public int VisibleCount(ByChain chain, int waitSecond = 30)
        {
            return Projections(chain, c => auto.WaitAll.ForVisible(c, -1), e => 1, waitSecond, "Count").Sum();
        }

        private TResult Projection<TResult>(ByChain chain, Func<ByChain, IWebElement> selector, Func<IWebElement, TResult> projector, int waitSecond = 30, string projectorText = null)
        {
            return Project(chain, selector, e => LogProjection(projector(e)), waitSecond, projectorText);
        }

        public TResult Projection<TResult>(ByChain chain, Func<IWebElement, TResult> projector, int waitSecond = 30, string projectorText = null)
        {
            return Project(chain, c => auto.Wait.ForLoad(c, -1), e => LogProjection(projector(e)), waitSecond, projectorText);
        }

        public List<TResult> Projections<TResult>(ByChain chain, Func<IWebElement, TResult> projector, int waitSecond = 30, string projectorText = null)
        {
            return Project(chain, c => auto.WaitAll.ForLoad(c, -1), elements => LogProjection(elements.Select(projector).ToList()), waitSecond, projectorText);
        }

        private List<TResult> Projections<TResult>(ByChain chain, Func<ByChain, List<IWebElement>> selector, Func<IWebElement, TResult> projector, int waitSecond = 30, string projectorText = null)
        {
            return Project(chain, selector, elements => LogProjection(elements.Select(projector).ToList()), waitSecond, projectorText);
        }

        private List<TResult> LogProjection<TResult>(List<TResult> projectionResult)
        {
            auto.Context.LogTrace($"Projection result: ({string.Join(", ", projectionResult)})");
            return projectionResult;
        }

        private TResult LogProjection<TResult>(TResult projectionResult)
        {
            auto.Context.LogTrace($"Projection result: ({projectionResult})");
            return projectionResult;
        }

        private TResult Project<TSelectorResult, TResult>(ByChain chain, Func<ByChain, TSelectorResult> selector, Func<TSelectorResult, TResult> projector, int waitSecond = 30, string projectorText = null)
        {
            int timeout = waitSecond * 1000;

            var sw = new Stopwatch();
            sw.Start();

            bool found = false;
            bool successfull = false;
            int retry = 1;

            TResult result = default(TResult);
            do
            {
                var selectorResult = selector(chain);
                found = selectorResult != null;

                if (found)
                {
                    try
                    {
                        result = projector(selectorResult);
                        successfull = true;
                    }
                    catch (Exception)
                    {
                        successfull = false;
                    }
                }

                if (!successfull && sw.ElapsedMilliseconds < timeout)
                {
                    Thread.Sleep(500);

                    if (!found)
                    {
                        auto.Context.LogTrace($"Retry {retry} because Selector [{chain}] NotFound in {sw.ElapsedMilliseconds} ms");
                    }
                    else
                    {
                        auto.Context.LogTrace($"Retry {retry} because Selector [{chain}] Found BUT >> Projection [{projectorText ?? projector.ToString()}] NotSuccessfull in {sw.ElapsedMilliseconds} ms");
                    }

                    retry++;
                }
            } while (!successfull && sw.ElapsedMilliseconds < timeout);

            if (!successfull)
            {
                if (waitSecond >= 0)
                {
                    string errorMessage;
                    if (!found)
                    {
                        errorMessage = $"Selector [{chain}] NotFound in {sw.ElapsedMilliseconds} ms";
                    }
                    else
                    {
                        errorMessage = $"Selector [{chain}] Found BUT >> Projection [{projectorText ?? projector?.ToString()}] NotSuccessfull in {sw.ElapsedMilliseconds} ms";
                    }

                    auto.Context.LogError(errorMessage);
                    throw new WebAutomationTimeOutException(errorMessage);
                }
            }
            else
            {
                var resultMessage = $"Selector [{chain}] Found and Projection [{projectorText ?? projector?.ToString()}] Successfull in {sw.ElapsedMilliseconds} ms";
                auto.Context.LogDebug(resultMessage);
            }

            return result;
        }
    }
}