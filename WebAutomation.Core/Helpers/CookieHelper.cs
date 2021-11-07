using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class CookieHelper
    {
        private readonly IWebAutomation auto;

        public CookieHelper(IWebAutomation auto) => this.auto = auto;
        private ICookieJar Cookies => auto.WebDriver.Manage().Cookies;
        
        private Cookie GetCookieByName(string name) => Cookies.GetCookieNamed(name);
        public void AddCookie(string name, string value, DateTime? expiryDate = null, string path = null, string domain = null) => Cookies.AddCookie(new Cookie(name, value, domain, path, expiryDate));
        public void DeleteCookieByName(string name) => Cookies.DeleteCookieNamed(name);
        public void DeleteAllCookies() => Cookies.DeleteAllCookies();
        public ReadOnlyCollection<Cookie> GetCookies() => Cookies.AllCookies;
    }
}
