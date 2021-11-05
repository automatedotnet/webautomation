using System;
using System.Reflection;
using OpenQA.Selenium;
using WebAutomation.Core.Exceptions;

namespace WebAutomation.Core
{
    public static class WebDriverExtensions
    {
        public static Screenshot TakeScreenshot(this IWebDriver driver)
        {
            if (driver is ITakesScreenshot takesScreenShot)
                return takesScreenShot.GetScreenshot();

            if (driver is IHasCapabilities hasCapabilities)
            {
                if (!hasCapabilities.Capabilities.HasCapability(CapabilityType.TakesScreenshot) || !(bool)hasCapabilities.Capabilities.GetCapability(CapabilityType.TakesScreenshot))
                    throw new WebDriverException("Driver capabilities do not support taking screenshots");

                var executeMethodInfo = hasCapabilities.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);
                if (!(executeMethodInfo?.Invoke(hasCapabilities, new[] { (object)DriverCommand.Screenshot, null }) is Response response))
                    throw new WebAutomationException("Unexpected failure getting ScreenShot; response was not in the proper format.");

                return new Screenshot(response.Value.ToString());
            }

            throw new WebDriverException("Driver does not implement ITakesScreenshot or IHasCapabilities");
        }

        public static void ExecuteJavaScript(this IWebDriver driver, string script, params object[] args) => ExecuteJavaScriptInternal(driver, script, args);

        public static T ExecuteJavaScript<T>(this IWebDriver driver, string script, params object[] args)
        {
            object o = ExecuteJavaScriptInternal(driver, script, args);
            T obj = default(T);
            Type nullableType = typeof(T);
            if (o == null)
            {
                if (nullableType.IsValueType && Nullable.GetUnderlyingType(nullableType) == null)
                    throw new WebDriverException("Script returned null, but desired type is a value type");
            }
            else
            {
                obj = nullableType.IsInstanceOfType(o) ? (T)o : throw new WebDriverException("Script returned a value, but the result could not be cast to the desired type");
            }

            return obj;
        }

        private static object ExecuteJavaScriptInternal(IWebDriver driver, string script, object[] args)
        {
            if (driver is IJavaScriptExecutor javaScriptExecutor)
                return javaScriptExecutor.ExecuteScript(script, args);

            throw new WebDriverException("Driver does not implement IJavaScriptExecutor");
        }
    }
}