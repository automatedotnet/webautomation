namespace WebAutomation.Core.Helpers
{
    public class ScriptHelper
    {
        private readonly WebAutomation auto;
        
        public ScriptHelper(WebAutomation auto)
        {
            this.auto = auto;
        }

        public T Execute<T>(string script, params object[] args)
        {
            return auto.WebDriver.ExecuteJavaScript<T>(script, args);
        }

        /// <summary>
        /// Switch to the frame or iframe if the target element is inside it and executes JavaScript there.
        /// </summary>
        public T Execute<T>(ByChain byChain, string script, params object[] args)
        {
            auto.Find.Elements(byChain);
            return auto.WebDriver.ExecuteJavaScript<T>(script, args);
        }

        /// <summary>
        /// Switch to the frame or iframe if the target element is inside it and executes JavaScript there.
        /// </summary>
        public void Execute(ByChain byChain, string script, params object[] args)
        {
            auto.Find.Elements(byChain);
            auto.WebDriver.ExecuteJavaScript(script, args);
        }
    }
}