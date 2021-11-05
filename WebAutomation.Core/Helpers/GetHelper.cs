using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class GetHelper
    {
        private readonly IWebAutomation auto;

        public GetHelper(IWebAutomation auto) => this.auto = auto;

        public string Text(ByChain chain, int? waitSecond = null) => Projection(chain, e => string.Equals(e.TagName, "input", StringComparison.InvariantCultureIgnoreCase) ? e.GetAttribute("value") : e.Text, waitSecond, "Text");
        public List<string> Texts(ByChain chain, int? waitSecond = null) => Projections(chain, e => e.Text, waitSecond, "Text");
        public string CssValue(ByChain chain, string cssPropertyName, int? waitSecond = null) => Projection(chain, e => e.GetCssValue(cssPropertyName), waitSecond, $"CssValue({cssPropertyName})");
        public string Attribute(ByChain chain, string attributeName, int? waitSecond = null) => Projection(chain, e => e.GetAttribute(attributeName), waitSecond, $"Attribute({attributeName})");
        public List<string> Attributes(ByChain chain, string attributeName, int? waitSecond = null) => Projections(chain, e => e.GetAttribute(attributeName), waitSecond, $"Attribute({attributeName})");

        public int Count(ByChain chain, int? waitSecond = null) => GetIsVisibleList(chain, waitSecond).Count;
        public int VisibleCount(ByChain chain, int? waitSecond = null) => GetIsVisibleList(chain, waitSecond).Count(v => v);

        private List<bool> GetIsVisibleList(ByChain chain, int? waitSecond) => Projections(chain, e => e.Displayed, waitSecond, "IsDisplayed");
        
        public TProjection Projection<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int? waitSecond = null, string projectionText = null) => auto.Wait.ForProjections(byChain, projection, waitSecond, projectionText).Single;
        public List<TProjection> Projections<TProjection>(ByChain byChain, Func<IWebElement, TProjection> projection, int? waitSecond = null, string projectionText = null) => auto.Wait.ForProjections(byChain, projection, waitSecond, projectionText);
    }
}