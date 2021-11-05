using System;
using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class Waiters
    {
        private readonly WebElementWaiterInternal webElementWaiterInternal;

        public Waiters(IWebAutomation auto) => webElementWaiterInternal = new WebElementWaiterInternal(auto);

        public IWebElement ForElement(ByChain byChain, int? waitSecond = null)
        {
            return ForElements(byChain, waitSecond).Single;
        }
        
        public ResultCollection<IWebElement> ForElements(ByChain byChain, int? waitSecond = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, waitSecond: waitSecond, ensureSingleResult: false);
        }

        public IWebElement ForElementWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, int? waitSecond = null, string matcherText = null)
        {
            return ForElementsWithCondition(byChain, matcher, waitSecond, matcherText).Single;
        }
        
        public ResultCollection<IWebElement> ForElementsWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, int? waitSecond = null, string matcherText = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, matcher, waitSecond: waitSecond, matcherText: matcherText, ensureSingleResult: false);
        }

        public IWebElement ForAction(ByChain byChain, Action<IWebElement> action, int? waitSecond = null, string actionText = null)
        {
            return ForActions(byChain, action, waitSecond, actionText).Single;
        }
        
        public ResultCollection<IWebElement> ForActions(ByChain byChain, Action<IWebElement> action, int? waitSecond = null, string actionText = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, action: action, waitSecond: waitSecond, actionText: actionText, ensureSingleResult: false);
        }

        public IWebElement ForActionWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, Action<IWebElement> action, int? waitSecond = null, string matcherText = null, string actionText = null)
        {
            return ForActionsWithCondition(byChain, matcher, action, waitSecond, matcherText, actionText).Single;
        }

        public ResultCollection<IWebElement> ForActionsWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, Action<IWebElement> action, int? waitSecond = null, string matcherText = null, string actionText = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, matcher, action, waitSecond, matcherText, actionText, false);
        }

        public TProjection ForProjection<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int? waitSecond = null, string projectionText = null)
        {
            return ForProjections(byChain, projection, waitSecond, projectionText).Single;
        }

        public ResultCollection<TProjection> ForProjections<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int? waitSecond = null, string projectionText = null)
        {
            return webElementWaiterInternal.For(byChain, projection: projection, waitSecond: waitSecond, projectionText: projectionText, ensureSingleResult: false);
        }

        public TProjection ForProjectionWithCondition<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher, Func<IWebElement, TProjection> projection, int? waitSecond = null, string matcherText = null, string projectionText = null)
        {
            return ForProjectionsWithCondition(byChain, matcher, projection, waitSecond, matcherText, projectionText).Single;
        }

        public ResultCollection<TProjection> ForProjectionsWithCondition<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher, Func<IWebElement, TProjection> projection, int? waitSecond = null, string matcherText = null, string projectionText = null)
        {
            return webElementWaiterInternal.For(byChain, matcher, projection: projection, waitSecond: waitSecond, matcherText: matcherText, projectionText: projectionText, ensureSingleResult: false);
        }

        public IWebElement ForVisible(ByChain chain, int? waitSecond = null) => ForElementWithCondition(chain, e => e.Displayed, waitSecond, "IsDisplayed");
        public IWebElement ForInvisible(ByChain chain, int? waitSecond = null) => ForElementWithCondition(chain, e => !e.Displayed, waitSecond, "IsNotDisplayed");
        public IWebElement ForClickable(ByChain chain, int? waitSecond = null) => ForElementWithCondition(chain, e => e.Displayed && e.Enabled, waitSecond, "Clickable");
        public IWebElement ForAttribute(ByChain chain, string attribute, int? waitSecond = null) => ForElementWithCondition(chain, e => !string.IsNullOrEmpty(e.GetAttribute(attribute)), waitSecond, $"HasAttribute('{attribute}')");
        public IWebElement ForCssClassRemoved(ByChain chain, string className, int? waitSecond = null) => ForElementWithCondition(chain, e => !e.GetAttribute("class").Contains(className), waitSecond, $"ClassRemoved('{className}')");
        public IWebElement ForCssClass(ByChain chain, string className, int? waitSecond = null) => ForElementWithCondition(chain, e => e.GetAttribute("class").Contains(className), waitSecond, $"ClassAdded('{className}')");
        public IWebElement ForNotEmptyText(ByChain chain, int? waitSecond = null) => ForElementWithCondition(chain, e => !string.IsNullOrEmpty(e.Text), waitSecond, "NotEmptyText");
    }
}