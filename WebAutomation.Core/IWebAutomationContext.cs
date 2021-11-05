namespace WebAutomation.Core
{
    public interface IWebAutomationContext
    {
        /// <summary>
        /// Generic Log method
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="msg"></param>
        void Log(LogLevel logLevel, string msg);

        /// <summary>
        /// The name of the execution context. It is used for ScreenShot naming, and in Logging to refer to the actual context. (Eg.: TestName)
        /// </summary>
        string ExecutionContextName { get; }

        /// <summary>
        /// The directory where the Screenshots, ContextInformation Dumps will be saved
        /// </summary>
        string WorkingDirectory { get; }
    }
}