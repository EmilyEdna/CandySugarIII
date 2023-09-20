using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CandySugar.Com.Library.FFMPegFactory;
using CandySugar.Com.Library.FileWrite;


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
            return await FMFactory.Default(opt => opt.Thread(5).InputFile(Path.Combine(catalog, mp3File)).Quality(320).AudioCodec("libmp3lame"))
                .Output(Path.Combine(catalog, $"[High]{mp3File}")).RunAsync();
        }
        /// <summary>
        /// 图片转视频
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static async Task<bool> ImageToVideo(this List<string> fileName, string catalog)
        {
            //-framerate 0.3 设置帧率(控制每张图片播放时长 相当于每张图片播放3秒)
            //-f image2 1 指定的格式(图片合成视频用以下参数)
            //-r 15 指定输出每秒15帧
            //-s 1920*1080 分辨率
            //-y 关闭询问
            //-threads 4 多线程
            //-c:v libx264 -pix_fmt yuv420p 解码
            var videoPath = Path.Combine(catalog, "Video");
            if (!Directory.Exists(videoPath)) Directory.CreateDirectory(videoPath);
            return await FMFactory.ImagePipe(opt =>
            {
                opt.FrameRate(0.3).Thread(5).ConCat(fileName).VideoCodec("libx264")
                .VideoFormat("yuvj420p").Aspect(16, 9).Rate(60).Sreen(1920, 1080).Args("-b:v 5000k");
            }).Output(Path.Combine(videoPath, $"{Guid.NewGuid()}.{FileTypes.Mp4}")).RunAsync();
        }
        /// <summary>
        /// 图片转视频带音频
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="audioFile">音频的绝对路径</param>
        /// <param name="audioTime">音频时常</param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static async Task<bool> ImageToVideo(this List<string> fileName, string audioFile, string audioTime, string catalog)
        {
            var videoPath = Path.Combine(catalog, "Video");
            if (!Directory.Exists(videoPath)) Directory.CreateDirectory(videoPath);
            //-framerate 0.3 设置帧率(控制每张图片播放时长 相当于每张图片播放3秒)
            //-f image2 1 指定的格式(图片合成视频用以下参数)
            //-r 15 指定输出每秒15帧
            //-s 1920*1080 分辨率
            //-y 关闭询问
            //-threads 4 多线程
            //-c:v libx264 -pix_fmt yuv420p 解码
            return await FMFactory.ImagePipe(opt =>
            {
                opt.Loop(1).FrameRate(0.3).Thread(5).ConCat(fileName).InputFile(audioFile).Quality(320)
                .AudioCodec("libmp3lame").AudioTime(audioTime).VideoCodec("libx264")
                .VideoFormat("yuvj420p").Aspect(16, 9).Rate(60).Sreen(1920, 1080).Args("-b:v 5000k");
            }).Output(Path.Combine(videoPath, $"{Guid.NewGuid()}.{FileTypes.Mp4}")).RunAsync();
        }
        /// <summary>
        /// 下载M4S流转视频无声音
        /// </summary>
        /// <param name="m4path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<bool> M4Video(this string m4path, string file)
        {
            return await FMFactory.Default(opt => opt.Thread(5).Args(Cmd).InputFile(m4path)) .Output(file).RunAsync();
        }
        /// <summary>
        /// 下载M4S流转音频无画面
        /// </summary>
        /// <param name="m4path"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static async Task<bool> M4Audio(this string m4path, string file)
        {
            return await FMFactory.Default(opt => opt.Thread(5).Args(Cmd).InputFile(m4path).Quality(320).AudioCodec("libmp3lame")).Output(file).RunAsync();
        }
        /// <summary>
        /// M4S流音频画面合并
        /// </summary>
        /// <param name="file"></param>
        /// <param name="m4audio"></param>
        /// <param name="m4video"></param>
        /// <param name="useDown"></param>
        /// <returns></returns>
        public static async Task<bool> M4VAMerge(this string file, string m4audio, string m4video, bool useDown = false)
        {
            return await FMFactory.Default(opt => {
                opt.Thread(5).Args(useDown ? Cmd : "").InputFile(m4video)
                .Args(useDown ? Cmd : "").InputFile(m4audio)
                .VideoCodec("libx264").Codec("copy");
            }).Output(file).RunAsync();
        }
    }
}
