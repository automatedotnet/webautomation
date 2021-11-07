using System;
using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class AlertHelper
    {
        private readonly IWebAutomation auto;

        public AlertHelper(IWebAutomation auto)
        {
            this.auto = auto;
        }

        public string Text
        {
            get
            {
                auto.GenericWaiter.WaitFor(IsVisible, 15, "WaitForAlertVisible");
                return SwitchToAlert().Text;
            }
        }

        public void Accept(int waitSeconds = 15) => Do(a => a.Accept(), "Accept", waitSeconds);
        public void Dismiss(int waitSeconds = 15 ) => Do(a => a.Dismiss(), "Dismiss", waitSeconds);

        public bool IsVisible()
        {
            try
            {
                SwitchToAlert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private void Do(Action<IAlert> action, string actionName, int waitSeconds = 15)
        {
            try
            {
                auto.GenericWaiter.WaitFor(IsVisible, waitSeconds, "WaitForAlertVisible");
                action(SwitchToAlert());
                auto.Context.LogDebug($"Alert action {actionName} performed");
                auto.GenericWaiter.WaitFor(() => !IsVisible(), waitSeconds, "WaitForAlertNotVisible");
            }
            catch (NoAlertPresentException e)
            {
                auto.Context.LogWarning("The alert is disappeared in the meantime. " + e.Message);
                throw;
            }
            
        }

        private IAlert SwitchToAlert() => auto.WebDriver.SwitchTo().Alert();
    }
}