using System;
using System.Diagnostics;
using System.Threading;

namespace WebAutomation.Core.Helpers.Common
{
    public class GenericWaiter
    {
        private readonly Action<LogLevel, string> log;

        public GenericWaiter(Action<LogLevel, string> log) => this.log = log;

        public void WaitFor(Func<bool> func, int waitSecond = 15, string name = null, int pollTimeSeconds = 1)
        {
            int timeout = waitSecond * 1000;

            var sw = Stopwatch.StartNew();
            bool isSuccessful = false;

            do
            {
                try
                {
                    isSuccessful = func();
                }
                catch (Exception ex)
                {
                    log(LogLevel.Trace, $"Exception at executing Func at waiting: {ex.Message} ({ex.InnerException?.Message})");
                }

                if (!isSuccessful && sw.ElapsedMilliseconds < timeout)
                    Thread.Sleep(pollTimeSeconds * 1000);

            } while (!isSuccessful && sw.ElapsedMilliseconds < timeout);

            log(LogLevel.Debug, $"Waiting for [{name ?? func.ToString()}] {(!isSuccessful ? "Not" : "")}Successful in {sw.ElapsedMilliseconds} ms");

            if (!isSuccessful)
                throw new WebAutomationException($"Waiting for [{name ?? func.ToString()}] NotSuccessful in {sw.ElapsedMilliseconds} ms");
        }
    }
}