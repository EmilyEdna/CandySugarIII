using System.Collections.Generic;
using System.Configuration;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library.ReadFile
{
    public class AppReader
    {

        public static string AppRead(string key) => ConfigurationManager.AppSettings[key];

        public static Dictionary<string,string> AppRead()
        {
            Dictionary<string,string> Result = [];
            ConfigurationManager.AppSettings.AllKeys.ForEnumerEach(key =>
            {
                Result.Add(key, ConfigurationManager.AppSettings[key]);
            });
            return Result;
        }
    }
}
