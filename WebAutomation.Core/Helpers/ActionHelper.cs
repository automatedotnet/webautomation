using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace WebAutomation.Core.Helpers
{
    public class ActionHelper
    {
        private readonly IWebAutomation auto;
        private readonly WebElementWaiterInternal webElementWaiterInternal;
        public ActionHelper(WebAutomation auto)
        {
            this.auto = auto;
            webElementWaiterInternal = new WebElementWaiterInternal(auto);
        }

        public void Action(ByChain byChain, Action<IWebElement> action, int? waitSecond = null, string actionText = null)
        {
            var single = webElementWaiterInternal.ForWebElements(byChain, action: action, waitSecond: waitSecond, actionText: actionText).Single;
        }
        
        public void ActionWithCondition(ByChain byChain, Func<IWebElement, bool> matcher, Action<IWebElement> action, int? waitSecond = null, string matcherText = null, string actionText = null)
        {
            var single = webElementWaiterInternal.ForWebElements(byChain, matcher, action, waitSecond, matcherText, actionText).Single;
        }

        public void Click(ByChain chain, bool forceScrolling = false, bool skipClickabilityCheck = false, int? waitSecond = 30)
        {
            ActionWithCondition(chain, e => skipClickabilityCheck || (e.Displayed && e.Enabled),
                e =>
                {
                    if (forceScrolling)
                        ScrollTo(chain, waitSecond);
                    e.Click();
                }, waitSecond, "Clickable", "Click");
        }

        public void Submit(ByChain chain, int waitSecond = 30) => ActionWithCondition(chain, e => e.Displayed && e.Enabled, e => e.Submit(), waitSecond, "Clickable", "Submit");

        public void SetText(ByChain chain, string text, bool withClear = true, int? waitSecond = 30)
        {
            Action(chain, e =>
            {
                if (withClear)
                    e.Clear();
                e.SendKeys(text);
            }, waitSecond, $"SetText ({(withClear ? "Clear, " : "")}'{text}')");
        }

        public void SendKeys(ByChain chain, string text, int? waitSecond = 30) => SetText(chain, text, false, waitSecond);
        public void Clear(ByChain chain, int? waitSecond = 30) => Action(chain, e => { e.Clear(); }, waitSecond, $"Clear {chain}");
        public void Action(ByChain chain, Action<IWebElement> action, string actionName = "CustomAction", int? waitSecond = 30) => Action(chain, action, waitSecond, actionName);

        public void DropFile(ByChain byChain, string filePath, int offsetX = 0, int offsetY = 0, int? waitSecond = 30)
        {
            Action(byChain, element =>
            {
                string dropFileJavaScript = @"
                    var target = arguments[0],
                        offsetX = arguments[1],
                        offsetY = arguments[2],
                        document = target.ownerDocument || document,
                        window = document.defaultView || window;

                    var input = document.createElement('INPUT');
                    input.type = 'file';
                    input.style.display = 'none';
                    input.onchange = function () {
                      target.scrollIntoView(true);

                      var rect = target.getBoundingClientRect(),
                          x = rect.left + (offsetX || (rect.width >> 1)),
                          y = rect.top + (offsetY || (rect.height >> 1)),
                          dataTransfer = { files: this.files };

                      ['dragenter', 'dragover', 'drop'].forEach(function (name) {
                        var evt = document.createEvent('MouseEvent');
                        evt.initMouseEvent(name, !0, !0, window, 0, 0, 0, x, y, !1, !1, !1, !1, 0, null);
                        evt.dataTransfer = dataTransfer;
                        target.dispatchEvent(evt);
                      });

                      setTimeout(function () { document.body.removeChild(input); }, 25);
                    };
                    document.body.appendChild(input);
                    return input;
                ";

                var input = auto.Script.Execute<IWebElement>(byChain, dropFileJavaScript, element, offsetX, offsetY);
                input.SendKeys(filePath);

                new WebDriverWait(auto.WebDriver, TimeSpan.FromSeconds(waitSecond ?? auto.Configuration.DefaultWaitTimeoutSeconds)).Until(StalenessOf(input));
            }, waitSecond, "DropFile");
        }

        public void ScrollTo(ByChain chain, int? waitSecond = 30)
        {
            Action(chain, e =>
            {
                //ToDo: Verify if it is still true. (Test for it)
                //Note: In case of FireFox the scrolling does not work, so we have to do it via JS directly [2017.10.17]
                if (!auto.Browser.IsChromeBrowser)
                    auto.Script.Execute(chain, "arguments[0].scrollIntoView(true);", e);
                else
                    new Actions(auto.WebDriver).MoveToElement(e).Perform();
            }, waitSecond, "ScrollTo");
        }

        private static Func<IWebDriver, bool> StalenessOf(IWebElement element)
        {
            return _ =>
            {
                try
                {
                    // Calling any method forces a staleness check
                    return element is not { Enabled: true };
                }
                catch (StaleElementReferenceException)
                {
                    return true;
                }
            };
        }
    }
}