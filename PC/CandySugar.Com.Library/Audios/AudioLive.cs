using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.Audios
{
    public class AudioLive:PropertyChangedBase
    {
        private string _LiveSpan;
        /// <summary>
        /// 实时时常 例如04:20
        /// </summary>
        public string LiveSpan
        {
            get => _LiveSpan;
            set => SetAndNotify(ref _LiveSpan, value);
        }
        private double _LiveSeconds;
        /// <summary>
        /// 实时秒
        /// </summary>
        public double LiveSeconds
        {
            get => _LiveSeconds;
            set => SetAndNotify(ref _LiveSeconds, value);
        }
        private ObservableCollection<double> _LiveData;
        /// <summary>
        /// 实时通道数据
        /// </summary>
        public ObservableCollection<double> LiveData
        {
            get => _LiveData;
            set => SetAndNotify(ref _LiveData, value);
        }
    }
}
