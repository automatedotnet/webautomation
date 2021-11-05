using System;

namespace WebAutomation.Core.Exceptions
{
    public class WebAutomationInfrastructureException : WebAutomationException
    {
        public WebAutomationInfrastructureException(string message)
            : base(message)
        {
        }

        public WebAutomationInfrastructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}