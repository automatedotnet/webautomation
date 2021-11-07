using OpenQA.Selenium;

namespace WebAutomation.Core.Helpers
{
    public class ScriptHelper
    {
        private readonly IWebAutomation auto;
        
        public ScriptHelper(IWebAutomation auto) => this.auto = auto;

        /// <summary>
        /// Locate the root "html" tag and execute the script in that frame.
        /// </summary>
        public T Execute<T>(string script, params object[] args) => Execute<T>(By.TagName("html"), script, args);

        /// <summary>
        /// Locate the byChain and execute the script in that frame.
        /// </summary>
        public T Execute<T>(ByChain byChain, string script, params object[] args)
        {
            T result = default(T);
            auto.Do.Action(byChain, _ => result = auto.WebDriver.ExecuteJavaScript<T>(script, args), null, "ExecuteJavaScript");
            return result;
        }

        /// <summary>
        /// Locate the byChain and execute the script in that frame.
        /// </summary>
        public void Execute(ByChain byChain, string script, params object[] args) => auto.Do.Action(byChain, _ => auto.WebDriver.ExecuteJavaScript(script, args), null, "ExecuteJavaScript");
    }
}