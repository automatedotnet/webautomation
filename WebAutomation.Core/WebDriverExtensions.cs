using System;
using System.Reflection;
using OpenQA.Selenium;

namespace WebAutomation.Core
{
    public static class WebDriverExtensions
    {
        public static Screenshot TakeScreenShot(this IWebDriver driver)
        {
            switch (driver)
            {
                case ITakesScreenshot takesScreenShot:
                    return takesScreenShot.GetScreenshot();
                case IHasCapabilities hasCapabilities:
                    if (!hasCapabilities.Capabilities.HasCapability(CapabilityType.TakesScreenshot) || !(bool)hasCapabilities.Capabilities.GetCapability(CapabilityType.TakesScreenshot))
                        throw new WebDriverException("Driver capabilities do not support taking screenshots");
                    if (!(driver.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object)driver, new object[2]
                    {
                        (object) DriverCommand.Screenshot,
                        null
                    }) is Response response2))
                        throw new WebDriverException("Unexpected failure getting screenshot; response was not in the proper format.");
                    return new Screenshot(response2.Value.ToString());
                default:
                    throw new WebDriverException("Driver does not implement ITakesScreenshot or IHasCapabilities");
            }
        }

        public static void ExecuteJavaScript(
            this IWebDriver driver,
            string script,
            params object[] args)
        {
            WebDriverExtensions.ExecuteJavaScriptInternal(driver, script, args);
        }
        
        public static T ExecuteJavaScript<T>(
            this IWebDriver driver,
            string script,
            params object[] args)
        {
            object o = WebDriverExtensions.ExecuteJavaScriptInternal(driver, script, args);
            T obj = default(T);
            Type nullableType = typeof(T);
            if (o == null)
            {
                if (nullableType.IsValueType && Nullable.GetUnderlyingType(nullableType) == (Type)null)
                    throw new WebDriverException("Script returned null, but desired type is a value type");
            }
            else
                obj = nullableType.IsInstanceOfType(o) ? (T)o : throw new WebDriverException("Script returned a value, but the result could not be cast to the desired type");
            return obj;
        }

        private static object ExecuteJavaScriptInternal(
            IWebDriver driver,
            string script,
            object[] args)
        {
            if (!(driver is IJavaScriptExecutor javaScriptExecutor))
                throw new WebDriverException("Driver does not implement IJavaScriptExecutor");
            return javaScriptExecutor.ExecuteScript(script, args);
        }
    }
}