using System;

namespace WebAutomation.Core.Exceptions
{
    public class WebAutomationElementNotFoundException : WebAutomationException
    {
        public WebAutomationElementNotFoundException(string message)
            : base(message)
        {
        }

        public WebAutomationElementNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}