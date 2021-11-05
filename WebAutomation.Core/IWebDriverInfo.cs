using System;
using OpenQA.Selenium;

namespace WebAutomation.Core
{
    public interface IWebDriverInfo
    {
        IWebDriver WebDriver { get; }
        BrowserType BrowserType { get; }
        Uri HubUri { get; }
        string DownloadFolder { get; set; }
    }
}