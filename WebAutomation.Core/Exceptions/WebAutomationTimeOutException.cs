using System;

namespace WebAutomation.Core.Exceptions
{
    public class WebAutomationTimeOutException : WebAutomationException
    {
        public WebAutomationTimeOutException(string message)
            : base(message)
        {
        }

        public WebAutomationTimeOutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}