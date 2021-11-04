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
            chromeOptions.AddArgument("disable-infobars");  //Does not seems to be working

            var chromeDriver = new ChromeDriver(chromeOptions);

            //ToDo: At browser open a Chrome consent page comes up. Auto accept or some disable setting is needed.

            chromeDriver.Navigate().GoToUrl("http://www.google.com");
            chromeDriver.FindElement(By.Id("q")).SendKeys("alma");
            chromeDriver.Quit();
        }
    }
}
