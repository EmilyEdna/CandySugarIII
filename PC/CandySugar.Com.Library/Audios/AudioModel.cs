using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.Audios
{
    public class AudioModel : PropertyChangedBase
    {
        private string _TimeSpan;
        /// <summary>
        /// 时常 例如04:20
        /// </summary>
        public string TimeSpan
        {
            get => _TimeSpan;
            set => SetAndNotify(ref _TimeSpan, value);
        }
        private double _Seconds;
        /// <summary>
        /// 时常
        /// </summary>
        public double Seconds
        {
            get => _Seconds;
            set => SetAndNotify(ref _Seconds, value);
        }
    }
}
