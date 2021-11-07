namespace WebAutomation.Core.Helpers
{
    public class BrowserHelper
    {
        private readonly IWebAutomation auto;

        public BrowserHelper(IWebAutomation auto) => this.auto = auto;

        public BrowserType BrowserType => auto.WebDriverInfo.BrowserType;

        public bool IsChromeBrowser => auto.WebDriverInfo.BrowserType == BrowserType.Chrome;
        public bool IsFirefoxBrowser => auto.WebDriverInfo.BrowserType == BrowserType.Firefox;
        public bool IsEdgeBrowser => auto.WebDriverInfo.BrowserType == BrowserType.Edge;


        public string Url => auto.WebDriver.Url;
        public string Title => auto.WebDriver.Title;
    }
}