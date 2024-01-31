using Sdk.Component.Vip.Image.sdk.ViewModel.Response;
using Sdk.Component.Vip.Wallhav.sdk.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Cosplay.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private List<CosplayInitElementResult> LabBuilder;
        private List<CosplayInitElementResult> LandBuilder;
        private List<string> RealLocal;
        private List<MenuInfo> Default = new List<MenuInfo> {
            new MenuInfo { Key = 3, Value = "下载选中" },
            new MenuInfo { Key = 4, Value = "删除选中" },
            new MenuInfo { Key = 5, Value = "无声相册" },
            new MenuInfo { Key = 6, Value = "音乐相册" }
        };

        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<CosplayLabView>();
            MenuIndex = new()
            {
                new MenuInfo { Key = 1, Value = "Lab" },
                new MenuInfo { Key = 2, Value = "Land" }
            };
            GenericDelegate.HandleAction = new(obj =>
            {
                if (obj is List<CosplayInitElementResult> input)
                {
                    if (input.First().Platform == PlatformEnum.Lab)
                    {
                        LabBuilder = input;
                        if (LabBuilder.Count >= 1)
                        {
                            if (!MenuIndex.Any(t => t.Key == 3 || t.Key == 4 || t.Key == 5 || t.Key == 6))
                                Default.ForEach(item => MenuIndex.Add(item));
                        }
                        else
                            Default.ForEach(item => MenuIndex.Remove(item));
                    }
                    else
                    {
                        LandBuilder = input;
                        if (LandBuilder.Count >= 1)
                        {
                            if (!MenuIndex.Any(t => t.Key == 3 || t.Key == 4 || t.Key == 5 || t.Key == 6))
                                Default.ForEach(item => MenuIndex.Add(item));
                        }
                        else
                            Default.ForEach(item => MenuIndex.Remove(item));
                    }
                }
            });
        }

        #region Field
        private double Width;
        private double Height;
        #endregion

        #region Property
        private Control _ComponentControl;
        public Control ComponentControl
        {
            get => _ComponentControl;
            set => SetAndNotify(ref _ComponentControl, value);
        }
        private ObservableCollection<MenuInfo> _MenuIndex;
        /// <summary>
        /// 平台菜单
        /// </summary>
        public ObservableCollection<MenuInfo> MenuIndex
        {
            get => _MenuIndex;
            set => SetAndNotify(ref _MenuIndex, value);
        }
        #endregion

        #region Command
        public void ActiveCommand(int key)
        {
            if (key == 1)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ComponentControl = Module.IocModule.Resolve<CosplayLabView>();
                    NotifyScreen();
                });
            if (key == 2)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ComponentControl = Module.IocModule.Resolve<CosplayLandView>();
                    NotifyScreen();
                });
            if (key == 3)
                DownSelectPicture();
            if (key == 4)
                RemoveSelectPicture();
            if (key == 5)
                BuilderVideoPicture();
            if (key == 6)
                BuilderVideoAudioPicture();
        }
        #endregion

        #region Method
        private void BuilderVideoPicture()
        {

        }
        private void BuilderVideoAudioPicture()
        {

        }
        private void DownSelectPicture()
        {

        }
        private void RemoveSelectPicture()
        {

        }
        public void NotifyScreen(double width, double height)
        {
            this.Width = width;
            this.Height = height;
            NotifyScreen();
        }
        private void NotifyScreen()
        {
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                ControlParam = Tuple.Create(this.Width, this.Height)
            });
        }
        #endregion
    }
}
