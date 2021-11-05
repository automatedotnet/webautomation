using System;
using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public static class IWebElementExtensions
    {
        /// <summary>
        /// element is iframe
        /// </summary>
        public static bool IsFrame(this IWebElement element)
            => string.Equals(element.TagName, "iframe", StringComparison.InvariantCultureIgnoreCase)
               || string.Equals(element.TagName, "frame", StringComparison.InvariantCultureIgnoreCase);
    }
}