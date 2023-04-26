using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace CandySugar.Com.Library.FileWrite
{
    public static class DownUtil
    {
        public static string FilePath(string fileName, string fileType, string component = "")
        {
            var catalog = Path.Combine(CommonHelper.DownloadPath, component);
            var files = Path.Combine(catalog, $"{fileName}.{fileType}");
            return files;
        }
        /// <summary>
        /// 文件写入
        /// </summary>
        /// <param name="result"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="component"></param>
        /// <param name="invoke"></param>
        public static void FileCreate(this byte[] result, string fileName, string fileType, string component = "", Action<string,string> invoke = null)
        {
            var catalog = SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, component));
            var files = SyncStatic.CreateFile(Path.Combine(catalog, $"{fileName}.{fileType}"));
            SyncStatic.WriteFile(result, files);
            Application.Current.Dispatcher.Invoke(() =>
            {
                invoke?.Invoke(catalog, $"{fileName}.{fileType}");
            });
        }
        /// <summary>
        /// 删除后在写入
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="component"></param>
        /// <param name="invoke"></param>
        public static void DeleteAndCreate<T>(this T data, string fileName, string fileType, string component = "", Action<string> invoke = null)
        {
            var catalog = Path.Combine(CommonHelper.DownloadPath, component);
            var files = Path.Combine(catalog, $"{fileName}.{fileType}");
            SyncStatic.DeleteFile(files);
            SyncStatic.CreateFile(files);
            SyncStatic.WriteFile(Encoding.UTF8.GetBytes(data.ToJson()), files);
            Application.Current.Dispatcher.Invoke(() =>
            {
                invoke?.Invoke(catalog);
            });
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T ReadFile<T>(string fileName, string fileType, string component = "")
        {
            if (!FileExists(fileName, fileType, component)) return default;
            var catalog = SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, component));
            var files = Path.Combine(catalog, $"{fileName}.{fileType}");
            var data = SyncStatic.ReadFile(files);
            if (data.IsNullOrEmpty()) return default;
            return data.ToModel<T>();
        }
        /// <summary>
        /// 文件是否存在 True 存在 False不存在
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool FileExists(string fileName, string fileType, string component = "")
        {
            return File.Exists(FilePath(fileName, fileType, component));
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="component"></param>
        public static void FileDelete(string fileName, string fileType, string component = "")
        {
            var stay = FileExists(fileName, fileType, component);
            if (stay) File.Delete(FilePath(fileName, fileType, component));
        }
    }
}
