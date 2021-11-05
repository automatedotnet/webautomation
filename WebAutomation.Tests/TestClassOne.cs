using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebAutomation.Tests
{
    [TestFixture]
    public class TestClassOne
    {
        [Test]
        public void TestOne()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddExcludedArgument("enable-automation");

            var chromeDriver = new ChromeDriver(chromeOptions);
            IJavaScriptExecutor js = (IJavaScriptExecutor)chromeDriver;
            
            chromeDriver.Navigate().GoToUrl("http://www.google.com");
            js.ExecuteScript("document.querySelector('div[aria-modal]').remove();");
            chromeDriver.FindElement(By.Name("q")).SendKeys("alma");
            chromeDriver.Quit();
        }
    }
}
