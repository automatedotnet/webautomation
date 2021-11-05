namespace WebAutomation.Core
{
    public interface IWebDriverFactory
    {
        IWebDriverInfo Create(IWebAutomationContext context);
    }
}