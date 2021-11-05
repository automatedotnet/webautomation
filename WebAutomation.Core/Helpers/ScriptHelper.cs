using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class ScriptHelper
    {
        private readonly WebAutomation auto;
        
        public ScriptHelper(WebAutomation auto) => this.auto = auto;

        /// <summary>
        /// Locate the root "html" tag and execute the script in that frame.
        /// </summary>
        public T Execute<T>(string script, params object[] args)
        {
            return Execute<T>(By.TagName("html"), script, args);
        }

        /// <summary>
        /// Locate the byChain and execute the script in that frame.
        /// </summary>
        public T Execute<T>(ByChain byChain, string script, params object[] args)
        {
            T result = default(T);
            auto.Wait.ForAction(byChain, _ => result = auto.WebDriver.ExecuteJavaScript<T>(script, args), null, "ExecuteJavaScript");
            return result;
        }

        /// <summary>
        /// Locate the byChain and execute the script in that frame.
        /// </summary>
        public void Execute(ByChain byChain, string script, params object[] args)
        {
            auto.Wait.ForAction(byChain, _ => auto.WebDriver.ExecuteJavaScript(script, args), null, "ExecuteJavaScript");
        }
    }
}