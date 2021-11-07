using System;
using System.IO;
using System.Runtime.Serialization;
using OpenQA.Selenium;
using WebAutomation.Core.Helpers.Common;

namespace WebAutomation.Core.Helpers
{
    public class EnvironmentHelper
    {
        private readonly IWebAutomation auto;

        public EnvironmentHelper(IWebAutomation auto)
        {
            this.auto = auto;
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
        }

        private string LogDirectory => Path.Combine(auto.Context.WorkingDirectory, "Logs");

        public void TakeScreenShot(string name = null, bool includeTimeStamp = false)
        {
            var timeStamp = DateTime.Now.ToString("HH-mm-ss-fff");
            var screenShotName = string.IsNullOrEmpty(name) ? string.Empty : "." + name;
            if (includeTimeStamp || string.IsNullOrEmpty(name))
                screenShotName = "." + timeStamp;

            if (auto.IsDisposed)
                return;

            if (auto.Alert.IsVisible())
            {
                auto.Context.LogDebug("Open alert is detected! It will be closed before Screenshot!");
                try
                {
                    auto.Alert.Dismiss();
                }
                catch (WebAutomationException e)
                {
                    auto.Context.LogWarning("The alert is disappeared in the meantime. " + e.Message);
                }
            }

            var fileName = $"{auto.Context.ExecutionContextName}{screenShotName}.Browser.png";
            var fullPathFileName = Path.Combine(LogDirectory, fileName);
            auto.WebDriver.TakeScreenshot().SaveAsFile(fullPathFileName, ScreenshotImageFormat.Png);
            auto.Context.LogInfo($"Browser screenshot was taken #img('{fileName}')");
        }

        public void LogContextInformation(string name)
        {
            try
            {
                TakeScreenShot(name);

                var fileName = $"{auto.Context.ExecutionContextName}{(string.IsNullOrEmpty(name) ? string.Empty : "." + name)}.html";
                var fullPathFileName = Path.Combine(LogDirectory, fileName);

                //ToDo: Read page source with JS, to get the actual HTML, not only the loaded one!

                File.WriteAllText(fullPathFileName, auto.WebDriver.PageSource);
                auto.Context.LogInfo($"PageSource was saved #lnk('{fileName}')");
            }
            catch (Exception e)
            {
                auto.Context.LogWarning($"Error during log context information: {e.Message}");
            }
        }

        public string SessionId => auto.WebDriver.SessionId();
    }
}