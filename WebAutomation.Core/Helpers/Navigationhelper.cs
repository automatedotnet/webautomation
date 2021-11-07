using System;
using OpenQA.Selenium;
using WebAutomation.Core.Helpers.Common;

namespace WebAutomation.Core.Helpers
{
    public class NavigationHelper
    {
        private readonly IWebAutomation auto;

        public NavigationHelper(IWebAutomation auto) => this.auto = auto;
        
        private INavigation Navigate => auto.WebDriver.Navigate();

        public void GoToUrl(string url, Credential useWinAuthCred = null) => GoToUrl(string.IsNullOrEmpty(url) ? null : new Uri(url), useWinAuthCred);
        public void GoToUrl(Uri url, Credential useWinAuthCred = null)
        {
            if (url == null)
                throw new WebAutomationException("Navigation to an Empty Url is not allowed!");

            if (useWinAuthCred != null)
            {
                if(!auto.Browser.IsChromeBrowser)
                    throw new WebAutomationException("Windows Authentication is supported only with Chrome!");
                
                url = new Uri(url.ToString().Replace("://", $"://{useWinAuthCred.UserName}:{useWinAuthCred.Password}@"));
            }

            Navigate.GoToUrl(url);
            auto.Context.Log($"Navigated to {url}. ");
        }

        public void Back() => Navigate.Back();
        public void Forward() => Navigate.Forward();
        public void Refresh() => Navigate.Refresh();
    }
}