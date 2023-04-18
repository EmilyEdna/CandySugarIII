using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library
{
    public class CommonHelper
    {
        /// <summary>
        /// 程序目录
        /// </summary>
        public static string AppPath => AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 日志路径
        /// </summary>
        public static string LogPath => Path.Combine(AppPath, "Logs", "CandyLogs.log");
        /// <summary>
        /// FFMPEG路径
        /// </summary>
        public static string FFMPEG => Path.Combine(AppPath, "ffmpeg", "ffmpeg.exe");
        /// <summary>
        /// 配置目录
        /// </summary>
        public static string OptionPath => Path.Combine(AppPath,"Component");
        /// <summary>
        /// 下载目录
        /// </summary>
        public static string DownloadPath => Path.Combine(AppPath, "download");
        /// <summary>
        /// 查询错误消息
        /// </summary>
        public static string SearckWordErrorInfomartion => "查询条件不能为空!";
        /// <summary>
        /// 下载完成消息
        /// </summary>
        public static string DownloadFinishInformation => "文件下载完成，是否打开目录?";
        /// <summary>
        /// 文件转换完成消息
        /// </summary>
        public static string ConvertFinishInformation => "文件转换完成，是否打开目录?";
        /// <summary>
        /// 组件错误消息
        /// </summary>
        public static string ComponentErrorInformation => "组件内部异常，请查看日志!";
        /// <summary>
        /// 网络异常消息
        /// </summary>
        public static string InternetErrorInformation => "网络异常请检查当前网络是否连接成功!";
        /// <summary>
        /// 配置文件
        /// </summary>
        public static List<string> OptionFile = new List<string>
        {
            "Component.json",
            "SystemOption.json"
        };
    }
}
