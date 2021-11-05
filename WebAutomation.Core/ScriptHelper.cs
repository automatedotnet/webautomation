using OpenQA.Selenium.Support.Extensions;
using WebAutomation.Core.Utils;

namespace WebAutomation.Core.Framework.Helpers
{
    public class ScriptHelper
    {
        private readonly BrowserAutomation auto;

        public ScriptHelper(BrowserAutomation auto)
        {
            this.auto = auto;
        }

        public T Execute<T>(string script, params object[] args)
        {
            return auto.WebDriver.ExecuteJavaScript<T>(script, args);
        }

        public T Execute<T>(ByChain byChain, string script, params object[] args)
        {
            SwitchToTargetFrame(byChain);
            return auto.WebDriver.ExecuteJavaScript<T>(script, args);
        }

        public void Execute(ByChain byChain, string script, params object[] args)
        {
            SwitchToTargetFrame(byChain);
            auto.WebDriver.ExecuteJavaScript(script, args);
        }

        private void SwitchToTargetFrame(ByChain byChain)
        {
            auto.Window.SwitchToMainWindow();

            foreach (var by in byChain)
            {
                var element = auto.WebDriver.FindElement(by);

                //Note: If element is Frame than switch to it before running the next selectors
                if (element.IsFrame())
                {
                    auto.Window.SwitchToFrame(element);
                }
            }
        }
    }
}