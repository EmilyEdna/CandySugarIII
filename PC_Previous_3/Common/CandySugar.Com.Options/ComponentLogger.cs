using Sdk.Plugins;
using Serilog;
using System;
using System.Threading.Tasks;
using XExten.Advance.InternalFramework.Email;
using XExten.Advance.LogFramework;

namespace CandySugar.Com.Options
{
    public class ComponentLogger : ISdkLogger
    {
        public Task Log(string className, string methodInfo, Exception message)
        {
            XLog.Fatal(message, $"类【{className}】方法【{methodInfo}】");
            return Task.CompletedTask;
        }
    }
    public static class SdkComponentLogger
    {
        public static ILogger AddSdkLogger(this ILogger logger)
        {
            SdkOption.EnableLog = true;
            return logger;
        }

        public static ILogger AddEmailLogger(this ILogger logger)
        {
            SdkOption.EnableEmail = true;
            SdkOption.AcceptEmail = "1575890051@qq.com";
            EmailSetting.SetOption("smtp.qq.com", "847432003@qq.com", "odvbqicdusiobfed");
            return logger;
        }
    }
}
