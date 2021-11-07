namespace WebAutomation.Core.Helpers
{
    public class BrowserInfoHelper
    {
        private readonly IWebAutomation auto;

        public BrowserInfoHelper(IWebAutomation auto) => this.auto = auto;

        public BrowserType BrowserType => auto.WebDriverInfo.BrowserType;

        public bool IsChromeBrowser => auto.WebDriverInfo.BrowserType == BrowserType.Chrome;
        public bool IsFirefoxBrowser => auto.WebDriverInfo.BrowserType == BrowserType.Firefox;
        public bool IsEdgeBrowser => auto.WebDriverInfo.BrowserType == BrowserType.Edge;

        //ToDO: Decide if move to WindowHelper (?)
        public string Url => auto.WebDriver.Url;
        public string Title => auto.WebDriver.Title;
    }
}