using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library.ReadFile
{
    public class AppReader
    {

        /// <summary>
        /// 读取单个节点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AppRead(string key) => ConfigurationManager.AppSettings[key];

        /// <summary>
        /// 读取所有节点到字典中
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> AppRead()
        {
            Dictionary<string, string> Result = [];
            ConfigurationManager.AppSettings.AllKeys.ForEnumerEach(key =>
            {
                Result.Add(key, ConfigurationManager.AppSettings[key]);
            });
            return Result;
        }

        /// <summary>
        /// 更新节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyvalue"></param>
        /// <param name="file"></param>
        public static void UpdateAppConfig(string key, string keyvalue, string file)
        {
            var config = ConfigurationManager.OpenExeConfiguration(file);
            if (!config.AppSettings.Settings.AllKeys.FirstOrDefault(t => t.Equals(key)).IsNullOrEmpty())
                config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, keyvalue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
