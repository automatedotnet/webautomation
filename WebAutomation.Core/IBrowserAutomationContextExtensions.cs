using System;

namespace WebAutomation.Core
{
    public static class IWebAutomationContextExtensions
    {
        public static void Log(this IWebAutomationContext ctx, string msg) => ctx.Log(LogLevel.Info, msg);
        public static void LogInfo(this IWebAutomationContext ctx, string msg) => ctx.Log(LogLevel.Info, msg);
        public static void LogTrace(this IWebAutomationContext ctx, string msg) => ctx.Log(LogLevel.Trace, msg);
        public static void LogDebug(this IWebAutomationContext ctx, string msg) => ctx.Log(LogLevel.Debug, msg);
        public static void LogWarning(this IWebAutomationContext ctx, string msg) => ctx.Log(LogLevel.Warning, msg);
        public static void LogError(this IWebAutomationContext ctx, string msg) => ctx.Log(LogLevel.Error, msg);
        public static void LogException(this IWebAutomationContext ctx, Exception e, string msg = "Exception Occurred:") => ctx.Log(LogLevel.Error, $"{msg}{Environment.NewLine}{e}");
    }
}