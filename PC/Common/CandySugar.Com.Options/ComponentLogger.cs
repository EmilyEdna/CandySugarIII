using Sdk.Plugins;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.InternalFramework.Email;

namespace CandySugar.Com.Options
{
    public class ComponentLogger : ISdkLogger
    {
        public Task Log(string className, string methodInfo, Exception message)
        {
            Serilog.Log.Logger.Error(message, $"类【{className}】方法【{methodInfo}】");
            return Task.CompletedTask;
        }
    }
    public static class SdkComponentLogger
    {
        public static Logger AddSdkLogger(this Logger logger)
        {
            SdkOption.EnableLog = true;
            return logger;
        }

        public static Logger AddEmailLogger(this Logger logger)
        {
            SdkOption.EnableEmail = true;
            SdkOption.AcceptEmail = "1575890051@qq.com";
            EmailSetting.SetOption("smtp.qq.com", "847432003@qq.com", "odvbqicdusiobfed");
            return logger;
        }
    }
}
