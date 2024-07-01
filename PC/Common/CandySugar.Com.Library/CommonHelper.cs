using System;
using System.Collections.Generic;
using System.IO;

namespace CandySugar.Com.Library
{
    public class CommonHelper
    {
        public static string Version => "2.0.0.0";
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
        /// 本地播放器
        /// </summary>
        public static string PlayerHtml => Path.Combine(AppPath, "Assets", "Player.html");
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
        /// 程序版本
        /// </summary>
        public static string VersionErronInformation => "版本检查异常，检查Raw配置!";
        /// <summary>
        /// 升级提示
        /// </summary>
        public static string UpgradeInformation => "检测到新版本，3秒后开始是升级!";
        /// <summary>
        /// 完整校验
        /// </summary>
        public static string ProgramErronInformation => "程序完整检验失败，3秒后退出!";
        /// <summary>
        /// Cookie检验
        /// </summary>
        public static string CookieError => "读取Cookie失败，检查Cookie是否正确!";
        /// <summary>
        /// 下载数据等待
        /// </summary>
        public static string DownloadWait => "队列下载中请等待!";
        /// <summary>
        /// 配置文件
        /// </summary>
        public static List<string> OptionFile = new List<string>
        {
            "Component.json",
            "SystemOption.json",
            "SysFunction.json"
        };
    }
}
