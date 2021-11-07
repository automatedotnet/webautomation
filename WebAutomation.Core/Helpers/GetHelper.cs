using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using WebAutomation.Core.Helpers.Common;

namespace WebAutomation.Core.Helpers
{
    public class GetHelper
    {
        private readonly WebElementWaiterInternal webElementWaiterInternal;

        public GetHelper(IWebAutomation auto) => webElementWaiterInternal = new WebElementWaiterInternal(auto);

        public TProjection Projection<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int? waitSecond = null, string projectionText = null)
        {
            return Projections(byChain, projection, waitSecond, projectionText).Single;
        }

        public ResultCollection<TProjection> Projections<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int? waitSecond = null, string projectionText = null)
        {
            return webElementWaiterInternal.For(byChain, projection: projection, waitSecond: waitSecond, projectionText: projectionText);
        }

        public TProjection ForProjectionWithCondition<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher, Func<IWebElement, TProjection> projection, int? waitSecond = null, string matcherText = null, string projectionText = null)
        {
            return ProjectionsWithCondition(byChain, matcher, projection, waitSecond, matcherText, projectionText).Single;
        }

        public ResultCollection<TProjection> ProjectionsWithCondition<TProjection>(ByChain byChain, Func<IWebElement, bool> matcher, Func<IWebElement, TProjection> projection, int? waitSecond = null, string matcherText = null, string projectionText = null)
        {
            return webElementWaiterInternal.For(byChain, matcher, projection: projection, waitSecond: waitSecond, matcherText: matcherText, projectionText: projectionText);
        }

        public string Text(ByChain chain, int? waitSecond = null) => Projection(chain, e => string.Equals(e.TagName, "input", StringComparison.InvariantCultureIgnoreCase) ? e.GetAttribute("value") : e.Text, waitSecond, "Text");
        public List<string> Texts(ByChain chain, int? waitSecond = null) => Projections(chain, e => e.Text, waitSecond, "Text");
        public string CssValue(ByChain chain, string cssPropertyName, int? waitSecond = null) => Projection(chain, e => e.GetCssValue(cssPropertyName), waitSecond, $"CssValue({cssPropertyName})");
        public string Attribute(ByChain chain, string attributeName, int? waitSecond = null) => Projection(chain, e => e.GetAttribute(attributeName), waitSecond, $"Attribute({attributeName})");
        public List<string> Attributes(ByChain chain, string attributeName, int? waitSecond = null) => Projections(chain, e => e.GetAttribute(attributeName), waitSecond, $"Attribute({attributeName})");

        public int Count(ByChain chain, int? waitSecond = null) => GetIsVisibleList(chain, waitSecond).Count;
        public int VisibleCount(ByChain chain, int? waitSecond = null) => GetIsVisibleList(chain, waitSecond).Count(v => v);

        private List<bool> GetIsVisibleList(ByChain chain, int? waitSecond) => Projections(chain, e => e.Displayed, waitSecond, "IsDisplayed");
    }
}