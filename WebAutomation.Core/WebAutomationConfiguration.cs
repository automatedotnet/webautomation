namespace WebAutomation.Core
{
    public class WebAutomationConfiguration : IWebAutomationConfiguration
    {
        public int DefaultWaitTimeoutSeconds { get; set; } = 30;
    }
}