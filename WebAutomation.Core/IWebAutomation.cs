using System;
using OpenQA.Selenium;
using WebAutomation.Core.Helpers;
using WebAutomation.Core.Helpers.Common;

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
        
        FindHelper Find { get; }
        WaiterHelper Wait { get; set; }
        ActionHelper Do { get; set; }
        GetHelper Get { get; set; }
        ScriptHelper Script { get; set; }
        BrowserInfoHelper Browser { get; set; }
        WindowHelper Window { get; set; }
        AlertHelper Alert { get; set; }
        NavigationHelper Navigate { get; set; }
        EnvironmentHelper Environment { get; set; }

        GenericWaiter GenericWaiter { get; }

        bool IsDisposed { get; }
        IWebAutomationConfiguration Configuration { get; }
        void Quit();
        event EventHandler Disposed;
    }
}