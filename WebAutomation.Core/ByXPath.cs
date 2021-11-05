using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace WebAutomation.Core
{
    public class ByXPath : By
    {
        public By InnerBy { get; private set; }

        public string XPathToFind { get; private set; }

        public ByXPath(string xPathToFind)
        {
            this.XPathToFind = xPathToFind;
            this.InnerBy = By.XPath(xPathToFind);
            Description = "ByXPath(RE): " + xPathToFind;
        }

        public override IWebElement FindElement(ISearchContext context)
        {
            return this.InnerBy.FindElement(context);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            return this.InnerBy.FindElements(context);
        }

        public By GetInnerBy(bool relativePath = true)
        {
            if (relativePath && XPathToFind.StartsWith("//"))
                return By.XPath("." + XPathToFind);
            return InnerBy;
        }

        public static ByXPath TryFrom(By by)
        {
            var byXpath = @by as ByXPath;
            if (byXpath != null)
                return byXpath;
            var description = GetPropValue(by, "Description");
            if (description?.StartsWith("By.XPath: ") == true)
            {
                var xpathToFind = description.Replace("By.XPath: ", string.Empty);
                return new ByXPath(xpathToFind);
            }

            return null;
        }

        private static string GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(src, null) as string;
        }
    }
}
