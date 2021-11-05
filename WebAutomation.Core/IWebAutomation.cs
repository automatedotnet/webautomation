using System;
using OpenQA.Selenium;
using WebAutomation.Core.Helpers;

namespace WebAutomation.Core
{
    public interface IWebAutomation : IDisposable
    {
        IWebAutomationContext Context { get; }
        
        /// <summary>
        /// Prefer the usage of the wrapper functionality, use Selenium directly where it is necessary!
        /// </summary>
        IWebDriver WebDriver { get; }

        IWebDriverInfo WebDriverInfo { get; }
        
        ElementAccessors Find { get; }
        WindowHelper Window { get; set; }

        GenericWaiter GenericWaiter { get; }

        bool IsDisposed { get; }
        IWebAutomationConfiguration Configuration { get; }
        Waiters Wait { get; set; }
        void Quit();
        event EventHandler Disposed;
    }
}