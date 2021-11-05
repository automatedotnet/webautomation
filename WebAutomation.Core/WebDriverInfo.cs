using System;
using OpenQA.Selenium;

namespace WebAutomation.Core
{
    public class WebDriverInfo : IWebDriverInfo
    {
        public IWebDriver WebDriver { get; set; }
        public BrowserType BrowserType { get; set; }
        public Uri HubUri { get; set; }
        public string DownloadFolder { get; set; }
    }
}