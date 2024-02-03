using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.Enums
{
    public enum EMenu
    {
        About = 1,
        /// <summary>
        /// 视频播放器
        /// </summary>
        VLCPlayer,
        /// <summary>
        /// 音频播放器
        /// </summary>
        AudioPlayer,
        /// <summary>
        /// 音频转高音质
        /// </summary>
        AudioToHigh,
        /// <summary>
        /// 图片转视频
        /// </summary>
        ImgToVideo,
        /// <summary>
        /// 图片转视频带音频
        /// </summary>
        ImgToAudio,
        /// <summary>
        /// 系统配置
        /// </summary>
        SysOption
    }
}
