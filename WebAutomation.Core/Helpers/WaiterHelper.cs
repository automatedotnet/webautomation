using System;
using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class WaiterHelper
    {
        private readonly WebElementWaiterInternal webElementWaiterInternal;

        public WaiterHelper(IWebAutomation auto) => webElementWaiterInternal = new WebElementWaiterInternal(auto);

        public IWebElement ForElement(ByChain byChain, int? waitSecond = null) => ForElements(byChain, waitSecond).Single;

        public ResultCollection<IWebElement> ForElements(ByChain byChain, int? waitSecond = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, waitSecond: waitSecond);
        }

        public IWebElement ForElementWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, int? waitSecond = null, string matcherText = null)
        {
            return ForElementsWithCondition(byChain, matcher, waitSecond, matcherText).Single;
        }
        
        public ResultCollection<IWebElement> ForElementsWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, int? waitSecond = null, string matcherText = null)
        {
            return webElementWaiterInternal.ForWebElements(byChain, matcher, waitSecond: waitSecond, matcherText: matcherText);
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