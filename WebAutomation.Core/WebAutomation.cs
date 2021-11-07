using System;
using System.IO;
using OpenQA.Selenium;
using WebAutomation.Core.Helpers;

namespace WebAutomation.Core
{
    public class WebAutomation : IWebAutomation
    {
        public IWebAutomationConfiguration Configuration { get; }
        public IWebAutomationContext Context { get; }

        public IWebDriver WebDriver => WebDriverInfo.WebDriver;
        public IWebDriverInfo WebDriverInfo { get; }

        public WebAutomation(IWebDriverFactory webDriverFactory, IWebAutomationContext context) : this(webDriverFactory, context, new WebAutomationConfiguration())
        {
            Window = new WindowHelper(this);
            Find = new FindHelper(this);
            Wait = new WaiterHelper(this);
            Do = new ActionHelper(this);
            Script = new ScriptHelper(this);
            Browser = new BrowserHelper(this);
            Get = new GetHelper(this);
        }

        public WaiterHelper Wait { get; set; }
        public BrowserHelper Browser { get; set; }
        public ActionHelper Do { get; set; }
        public GetHelper Get { get; set; }
        public ScriptHelper Script { get; set; }
        public WindowHelper Window { get; set; }
        public FindHelper Find { get; set; }


        //IHasSessionId



        public WebAutomation(IWebDriverFactory webDriverFactory, IWebAutomationContext context, IWebAutomationConfiguration configuration)
        {
            WebDriverInfo = webDriverFactory.Create(context);

            Configuration = configuration;
            Context = context;

            if (!Directory.Exists(Context.WorkingDirectory))
                Directory.CreateDirectory(Context.WorkingDirectory);

            IsDisposed = false;

            GenericWaiter = new GenericWaiter(Context.Log);
        }

        public GenericWaiter GenericWaiter { get; }

        public bool IsDisposed { get; private set; }

        public virtual void Quit()
        {
            if (IsDisposed)
                return;

            WebDriver.Quit();
            IsDisposed = true;
            OnDisposed();
        }

        public void Dispose() => Quit();

        public event EventHandler Disposed;
        protected virtual void OnDisposed() => Disposed?.Invoke(this, EventArgs.Empty);
    }
}