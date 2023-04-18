using CandySugar.Com.Library.FileWrite;
using CliWrap;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace CandySugar.Com.Library.FFMPeg
{
    public static class FFMPegUtil
    {
        private static string Cmd = "-user_agent \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.39\" -headers \"referer:https://www.bilibili.com/\"";

        /// <summary>
        /// MP3音质提升
        /// </summary>
        /// <param name="mp3File"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static async Task<bool> Mp3ToHighMP3(this string mp3File, string catalog)
        {
            StringBuilder Info = new StringBuilder();

            var cmd = await Cli.Wrap(CommonHelper.FFMPEG)
                    .WithArguments($"-threads 5 -i {Path.Combine(catalog, mp3File)} -ab 320k -acodec libmp3lame  -y {Path.Combine(catalog, $"[High]{mp3File}")}")
                     .WithStandardErrorPipe(PipeTarget.ToStringBuilder(Info))
                     .ExecuteAsync();
            Log.Logger.Information(Info.ToString());
            return cmd.ExitCode == 0;
        }
        /// <summary>
        /// 图片转视频
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static async Task<bool> ImageToVideo(this List<string> fileName, string catalog)
        {
            var videoPath = Path.Combine(catalog, "Video");
            StringBuilder Info = new StringBuilder();
            if (!Directory.Exists(videoPath)) Directory.CreateDirectory(videoPath);
            //-framerate 0.3 设置帧率(控制每张图片播放时长 相当于每张图片播放3秒)
            //-f image2 1 指定的格式(图片合成视频用以下参数)
            //-r 15 指定输出每秒15帧
            //-s 1920*1080 分辨率
            //-y 关闭询问
            //-threads 4 多线程
            //-c:v libx264 -pix_fmt yuv420p 解码
            var args = $"-f image2pipe -framerate 0.3 -threads 5 -y -i \"concat:{string.Join("|", fileName)}\" -c:v libx264 -pix_fmt yuvj420p -aspect 16:9 -b:v 5000K -r 60 -s 1920*1080 {Path.Combine(videoPath, $"{Guid.NewGuid()}.{FileTypes.Mp4}")}";
            var cmd = await Cli.Wrap(CommonHelper.FFMPEG)
                .WithArguments(args)
                     .WithStandardErrorPipe(PipeTarget.ToStringBuilder(Info))
                     .ExecuteAsync();
            Log.Logger.Information(Info.ToString());
            return cmd.ExitCode == 0;
        }
        /// <summary>
        /// 图片转视频带音频
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="audioFile">音频的绝对路径</param>
        /// <param name="audioTime">音频时常</param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static async Task<bool> ImageToVideo(this List<string> fileName,string audioFile,string audioTime,  string catalog)
        {
            var videoPath = Path.Combine(catalog, "Video");
            StringBuilder Info = new StringBuilder();
            if (!Directory.Exists(videoPath)) Directory.CreateDirectory(videoPath);
            //-framerate 0.3 设置帧率(控制每张图片播放时长 相当于每张图片播放3秒)
            //-f image2 1 指定的格式(图片合成视频用以下参数)
            //-r 15 指定输出每秒15帧
            //-s 1920*1080 分辨率
            //-y 关闭询问
            //-threads 4 多线程
            //-c:v libx264 -pix_fmt yuv420p 解码
            var args = $"-loop 1 -f image2pipe -framerate 0.3 -threads 5 -y -i \"concat:{string.Join("|", fileName)}\" -i {audioFile} -ab 320k -acodec libmp3lame -t {audioTime} -c:v libx264 -pix_fmt yuvj420p -aspect 16:9 -b:v 5000K -r 60 -s 1920*1080 {Path.Combine(videoPath, $"{Guid.NewGuid()}.{FileTypes.Mp4}")}";
            var cmd = await Cli.Wrap(CommonHelper.FFMPEG)
                .WithArguments(args)
                     .WithStandardErrorPipe(PipeTarget.ToStringBuilder(Info))
                     .ExecuteAsync();
            Log.Logger.Information(Info.ToString());
            return cmd.ExitCode == 0;
        }
        /// <summary>
        /// 下载M4S流转视频无声音
        /// </summary>
        /// <param name="m4path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<bool> M4Video(this string m4path, string file)
        {
            StringBuilder Info = new StringBuilder();

            var cmd = await Cli.Wrap(CommonHelper.FFMPEG)
                    .WithArguments($"-threads 5 -y {Cmd} -i {m4path} {file}")
                     .WithStandardErrorPipe(PipeTarget.ToStringBuilder(Info))
                     .ExecuteAsync();
            Log.Logger.Information(Info.ToString());
            return cmd.ExitCode == 0;
        }
        /// <summary>
        /// 下载M4S流转音频无画面
        /// </summary>
        /// <param name="m4path"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static async Task<bool> M4Audio(this string m4path, string file)
        {
            StringBuilder Info = new StringBuilder();

            var cmd = await Cli.Wrap(CommonHelper.FFMPEG)
                    .WithArguments($"-threads 5 -y {Cmd} -i {m4path} -ab 320k -acodec libmp3lame {file}")
                     .WithStandardErrorPipe(PipeTarget.ToStringBuilder(Info))
                     .ExecuteAsync();
            Log.Logger.Information(Info.ToString());
            return cmd.ExitCode == 0;
        }
        /// <summary>
        /// M4S流音频画面合并
        /// </summary>
        /// <param name="file"></param>
        /// <param name="m4audio"></param>
        /// <param name="m4video"></param>
        /// <returns></returns>
        public static async Task<bool> M4VAMerge(this string file, string m4audio, string m4video)
        {
            StringBuilder Info = new StringBuilder();

            var cmd = await Cli.Wrap(CommonHelper.FFMPEG)
                    .WithArguments($"-threads 5 -y {Cmd} -i {m4video} {Cmd} -i {m4audio} -codec copy -c:v libx264 {file}")
                     .WithStandardErrorPipe(PipeTarget.ToStringBuilder(Info))
                     .ExecuteAsync();
            Log.Logger.Information(Info.ToString());
            return cmd.ExitCode == 0;
        }
    }
}
