using System;
using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class Waiters
    {
        private readonly IWebAutomation auto;
        private readonly bool ensureSingleResult;

        private readonly WebElementWaiterInternal webElementWaiterInternal;

        public Waiters(IWebAutomation auto, bool ensureSingleResult = true)
        {
            this.auto = auto;
            this.ensureSingleResult = ensureSingleResult;

            webElementWaiterInternal = new WebElementWaiterInternal(auto, ensureSingleResult);
        }

        public ResultCollection<IWebElement> ForLoad(ByChain byChain, int waitSecond = 30)
        {
            return webElementWaiterInternal.ForWebElements(byChain, null, null, waitSecond);
        }

        public ResultCollection<IWebElement> ForLoadWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, int waitSecond = 30, string matcherText = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, matcher, null, waitSecond, matcherText);
        }

        public ResultCollection<IWebElement> ForAction(ByChain byChain, Action<IWebElement> action, int waitSecond = 30, string actionText = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, null, action, waitSecond, null, actionText);
        }

        public ResultCollection<IWebElement> ForActionWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, Action<IWebElement> action, int waitSecond = 30, string matcherText = null, string actionText = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, matcher, action, waitSecond, matcherText, actionText);
        }

        public ResultCollection<TProjection> ForProjection<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int waitSecond = 30, string projectionText = null)
        {
            return webElementWaiterInternal.For(byChain, null, null, projection, waitSecond, null, null, projectionText);
        }
        
        public ResultCollection<TProjection> ForProjectionWithCondition<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher, Func<IWebElement, TProjection> projection, int waitSecond = 30, string matcherText = null, string projectionText = null)
        {
            return webElementWaiterInternal.For(byChain, matcher, null, projection, waitSecond, null, matcherText, projectionText);
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