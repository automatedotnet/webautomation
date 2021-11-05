using System.Linq;
using OpenQA.Selenium;

namespace WebAutomation.Core
{
    public class WindowHelper
    {
        private readonly IWebAutomation auto;

        public WindowHelper(IWebAutomation auto)
        {
            this.auto = auto;

            //Note: We save the initial window handle to be able to identify if we are in other Tab/Window/Frame
            currentWindowHandle = originalWindowHandle = auto.WebDriverInfo.WebDriver.CurrentWindowHandle;
        }

        private readonly string originalWindowHandle;
        private string currentWindowHandle;

        private string LastWindowHandle => auto.WebDriver.WindowHandles.Last(wh => wh != originalWindowHandle);
        private string FirstWindowHandle => auto.WebDriver.WindowHandles.First();

        /// <summary>
        /// Is the focus on the root Window and not on any of the child or sub-child Frame
        /// </summary>
        private bool isFocusOnMainWindow = true;

        /// <summary>
        /// If focus was moved away from current main window, then switches back to it. (Otherwise does not do anything)
        /// Not intended for external use!
        /// </summary>
        internal void SwitchToMainWindow()
        {
            //Note: This check is only an optimization to prevent unnecessary idempotent Switches to MainWindow (In case we are already there!)
            if (!isFocusOnMainWindow)
                SwitchToWindow(currentWindowHandle);
        }

        internal void SwitchToFrame(IWebElement frame)
        {
            auto.WebDriver.SwitchTo().Frame(frame);
            isFocusOnMainWindow = false;
        }

        public void OpenNewTab()
        {
            auto.WebDriver.ExecuteJavaScript("window.open()");
            SwitchToLastWindow();
        }

        public void CloseActualTab()
        {
            auto.WebDriver.ExecuteJavaScript("window.close()");
            SwitchToOriginalWindow();
        }

        public void SwitchToFirstWindow() => SwitchToWindow(FirstWindowHandle);
        public void SwitchToLastWindow() => SwitchToWindow(LastWindowHandle);

        public void SwitchToOriginalWindow()
        {
            if(auto.WebDriver.WindowHandles.Contains(originalWindowHandle))
                SwitchToWindow(originalWindowHandle);
            else
                SwitchToFirstWindow();
        }

        private void SwitchToWindow(string windowHandle)
        {
            currentWindowHandle = windowHandle;
            auto.WebDriver.SwitchTo().Window(currentWindowHandle);
            isFocusOnMainWindow = true;
        }
    }
}