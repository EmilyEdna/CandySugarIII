using CandySugar.Com.Library.Enums;
using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CandySugar.Com.Library.Audios
{
    public class AudioFactory
    {
        #region Singleton
        private static AudioFactory Instaned;
        public static AudioFactory Instance
        {
            get
            {
                if (Instaned == null)
                {
                    Instaned = new();
                    return Instaned;
                }
                else
                {
                    return Instaned;
                }
            }
        }
        #endregion

        #region Field
        /// <summary>
        /// 采样数据的对象锁，防止未分离左右通道就进入下一次采样
        /// </summary>
        private object SampleLock = new();
        /// <summary>
        /// 输出设备
        /// </summary>
        private WaveOutEvent WaveOutput;
        /// <summary>
        /// 音频读取 支持格式更多
        /// </summary>
        private MediaFoundationReader MediaReader;
        /// <summary>
        /// 音频读取
        /// </summary>
        public AudioFileReader AudioReader;
        /// <summary>
        /// 每个单声道数据样本的位数，例如 16位，24位，32位
        /// </summary>
        private int BitsChannelSample;
        /// <summary>
        /// 采样率，例如 44.1Khz ，就是 44100
        /// </summary>
        private int SampleRate;
        /// <summary>
        /// 通道数量
        /// </summary>
        private int ChannelCount;
        /// <summary>
        /// 音频时常信息
        /// </summary>
        private AudioModel AudioInfo;
        /// <summary>
        /// 音频采样长度
        /// </summary>
        private float[] SampleArray;
        /// <summary>
        /// 实时播放数据
        /// </summary>
        private Action<AudioLive> LiveAction;
        /// <summary>
        /// 实时波形倍率
        /// </summary>
        private double Pow;
        /// <summary>
        /// 显示的音阶
        /// </summary>
        private int Channal;
        #endregion

        #region ReadOnlyProperty
        /// <summary>
        /// 只读播放器属性
        /// </summary>
        public WaveOutEvent WaveOutReadOnly => WaveOutput;
        #endregion

        #region PublicMethod
        /// <summary>
        /// 初始化音频播放
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AudioFactory InitAudio(string path)
        {
            Dispose();

            WaveOutput = new();
            AudioReader = new(path);
            AudioInfo = new();

            BitsChannelSample = AudioReader.WaveFormat.BitsPerSample;
            SampleRate = AudioReader.WaveFormat.SampleRate;
            ChannelCount = AudioReader.WaveFormat.Channels;

            return this;
        }
        /// <summary>
        /// 初始化实时播放数据
        /// </summary>
        /// <param name="LiveAction"></param>
        /// <returns></returns>
        public AudioFactory InitLiveData(Action<AudioLive> LiveAction, int Channal = 32, double Pow = 10)
        {
            this.Pow = Pow;
            this.Channal = Channal;
            this.LiveAction = LiveAction;
            WasapiLoopbackCapture cap = new();
            cap.DataAvailable += FourierTransformEventAsync;
            cap.StartRecording();
            return this;
        }

        private void FourierTransformEventAsync(object sender, WaveInEventArgs e)
        {
            if (AudioReader != null)
            {
                SampleArray = Enumerable.Range(0, e.BytesRecorded / 150).Select(i => BitConverter.ToSingle(e.Buffer, i * 4)).ToArray();
                if (SampleArray.Length > 0)
                    LiveAction.Invoke(FourierTransform());
            }
        }

        /// <summary>
        /// 改变音量
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public AudioFactory ChangeVolume(float volume)
        {
            if (WaveOutput == null && AudioReader == null)
                throw new NullReferenceException("请先调用【InitAudio】方法!");
            WaveOutput.Volume = volume;
            return this;
        }
        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="action">返回的音频信息</param>
        /// <param name="module">模式</param>
        /// <returns></returns>
        public AudioFactory RunPlay(Action<AudioModel> action = null, EPlay module = EPlay.Play)
        {
            if (WaveOutput == null && AudioReader == null)
                throw new NullReferenceException("请先调用【InitAudio】方法!");
            if (module < EPlay.Stop && module > 0)
            {
                WaveOutput.Init(AudioReader);
                AudioInfo.Seconds = AudioReader.TotalTime.TotalSeconds;
                AudioInfo.TimeSpan = AudioReader.TotalTime.ToString().Split(".").FirstOrDefault().Substring(3, 5);
                if (module == EPlay.Play)
                {
                    WaveOutput.Play();
                }
                if (module == EPlay.Pause) WaveOutput.Pause();
            }
            else
                WaveOutput.Stop();

            action?.Invoke(AudioInfo);
            return this;
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 傅里叶变换FTF
        /// </summary>
        private AudioLive FourierTransform()
        {
            #region 分离左右通道
            //分离后的通道采样数据
            float[][] ChannelSimpleArray;
            //防止未分离完左右通道就进入下一次调用
            lock (SampleLock)
            {
                ChannelSimpleArray = Enumerable.Range(0, ChannelCount)//分离通道
                   .Select(channel => Enumerable.Range(0, SampleArray.Length / ChannelCount) //对每个通过的数据进行处理
                   .Select(item => SampleArray[channel + item * ChannelCount]).ToArray())//每个通道的数组长度 左右左右，这样读取
                   .ToArray();
            }
            #endregion

            #region 合并左右通道并取平均值
            float[] ChannelAverageSample = Enumerable.Range(0, ChannelSimpleArray[0].Length)
            //每次读取一个左右数据合并、取平均值
            .Select(item => Enumerable.Range(0, ChannelCount).Select(channel => ChannelSimpleArray[channel][item]).Average())
            .ToArray();
            #endregion

            #region 傅里叶变换
            //NAudio 提供了快速傅里叶变换的方法, 通过傅里叶变换, 可以将时域数据转换为频域数据
            // 取对数并向上取整
            int log = (int)Math.Ceiling(Math.Log(ChannelAverageSample.Length, 2));
            //对于快速傅里叶变换算法, 需要数据长度为 2 的 n 次方
            int length = (int)Math.Pow(2, log);
            float[] filledSample = new float[length];
            //拷贝到新数组
            Array.Copy(ChannelAverageSample, filledSample, ChannelAverageSample.Length);
            //将采样转化为复数
            Complex[] complexArray = filledSample
                .Select((value, index) => new Complex() { X = value })
                .ToArray();
            //进行傅里叶变换
            FastFourierTransform.FFT(false, log, complexArray);
            #endregion

            #region 提取需要的频域信息

            Complex[] halfComeplexArray = complexArray
                .Take(complexArray.Length / 2)//数据是左右对称的，所以只取一半
                .ToArray();

            //这个已经是频域数据了
            double[] resultArray = complexArray
                .Select(value => Math.Sqrt(value.X * value.X + value.Y * value.Y))//复数取模
                .ToArray();

            //我们取 最小频率 ~ 20000Hz
            //对于变换结果, 每两个数据之间所差的频率计算公式为 采样率/采样数, 那么我们要取的个数也可以由 20000 / (采样率 / 采样数) 来得出
            //当然，因为我这里并没有指定频率与幅值，所以顺便取几个数就行，若有需要可以再去细分各个频率的幅值
            int count = 44100 / (this.SampleRate / length);
            double[] finalData = resultArray.Take(count).ToArray();

            #endregion

            #region 获取实时播放时间和长度
            var CurrentSpan = AudioReader.CurrentTime.ToString().Split(".").FirstOrDefault().Substring(3, 5);
            var CurrentSeconds = AudioReader.CurrentTime.TotalSeconds;
            #endregion

            #region 设置绑定数据
            var LineData = finalData.Take(Channal).Select(t => t * Pow).ToList();
            #endregion

            return new AudioLive
            {
                LiveData = new ObservableCollection<double>(LineData),
                LiveSeconds = CurrentSeconds,
                LiveSpan = CurrentSpan
            };
        }
        public void Dispose()
        {
            AudioInfo = null;
            if (WaveOutput != null)
            {
                WaveOutput.Dispose();
                WaveOutput = null;
            }
            if (AudioReader != null)
            {
                AudioReader.Dispose();
                AudioReader = null;
            }
        }
        #endregion
    }
}
