using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using WebAutomation.Core.Diagnostic;

namespace WebAutomation.Core.Helpers
{
    public class FindHelper
    {
        private readonly IWebAutomation auto;

        public FindHelper(IWebAutomation auto) => this.auto = auto;

        public List<IWebElement> Elements(ByChain chain)
        {
            var chainResolutionList = new List<string>();
            try
            {
                List<IWebElement> elements = null;

                //Note: Switch to MainWindow (Because we don't know, what is the current Frame)
                chainResolutionList.Add("SwitchTo(MainWindow)");
                auto.Window.SwitchToMainWindow();

                foreach (var by in chain)
                {
                    if (elements == null)
                    {
                        chainResolutionList.Add($"FindElements('{by}')");
                        elements = auto.WebDriver.FindElements(by).ToList();
                    }
                    else
                    {
                        var frameNode = elements.FirstOrDefault(e => e.IsFrame());

                        //Note: If element is Frame than switch to it before running the next selectors
                        if (frameNode != null)
                        {
                            var previousBy = chain.ToArray()[chain.ToList().IndexOf(by) - 1];
                            if (elements.Count > 1)
                            {
                                auto.Context.LogWarning(
                                    $"Switch to frame is needed but other siblings found for By {previousBy} in locator chain. They will be not traversed.");
                            }

                            chainResolutionList.Add($"SwitchTo(Frame, '{previousBy}')");
                            auto.Window.SwitchToFrame(frameNode);

                            chainResolutionList.Add($"FindElements('{by}')");
                            elements = auto.WebDriver.FindElements(by).ToList();
                        }
                        else
                        {
                            chainResolutionList.Add($"PreviousElement.FindElements('{by}')");

                            var relativeBy = by;
                            if (chain.AutoFixXPath)
                            {
                                //Note: Fix non relative xpath in chain. (Search by XPath inside the element)
                                var byXpath = ByXPath.TryFrom(by);
                                if (byXpath != null)
                                    relativeBy = byXpath.GetInnerBy(true);
                            }

                            elements = elements.SelectMany(element => element.FindElements(relativeBy)).ToList();
                        }
                    }
                }

                //auto.Context.LogTrace("Successfully GetElement(s) for ByChain: " + string.Join(", ", chainResolutionList));

                return elements;
            }
            catch (StaleElementReferenceException e)
            {
                var errorMessage = $"Not successful GetElement(s) for ByChain: [{string.Join(", ", chainResolutionList)}] because StaleElementReferenceException.";
                auto.Context.LogWarning(errorMessage);

                return null;
            }
            catch (Exception e)
            {
                var errorMessage = $"Not successful GetElement(s) for ByChain: [{string.Join(", ", chainResolutionList)}] because Exception. The message was: {e.Message}";
                auto.Context.LogWarning(errorMessage);

                return null;
            }
        }
    }
}