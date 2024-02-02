using CandySugar.Com.Library.Audios;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CandySugar.MainUI.Views
{
    /// <summary>
    /// TestView.xaml 的交互逻辑
    /// </summary>
    public partial class TestView : Window
    {
        public TestView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AudioFactory.Instance.
                InitAudio("D:\\WorkSpace\\CandySugar\\PC\\CandySugar.MainUI\\bin\\Debug\\net8.0-windows\\download\\Bilibili\\Nightcore - 暗夜星河.mp3")
                .InitLiveData(opt => {
                    this.Dispatcher.Invoke(() =>
                    {
                        Lbox.ItemsSource = opt.LiveData;
                    });
                })
                .RunPlay();
            //var audioFile = new AudioFileReader(Pa.Text);
            //var outputDevice = new WaveOutEvent();
            //outputDevice.Init(audioFile);
            //outputDevice.Play();
        }
    }
}
