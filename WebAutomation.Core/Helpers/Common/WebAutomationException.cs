using System;

namespace WebAutomation.Core.Helpers.Common
{
    public class WebAutomationException : Exception
    {
        public WebAutomationException(string message)
            : base(message)
        {
        }

        public WebAutomationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}