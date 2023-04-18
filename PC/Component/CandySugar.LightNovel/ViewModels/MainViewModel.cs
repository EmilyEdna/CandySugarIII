using CandySugar.LightNovel.View;
using CommunityToolkit.Mvvm.Messaging;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CandySugar.LightNovel.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {

        public MainViewModel()
        {
            SdkLicense.Register(new SdkLicenseModel
            {
                Account = "EmilyEdna",
                Password = DateTime.Now.ToString("yyyyMMdd")
            });
            ComponentControl = Module.IocModule.Resolve<IndexView>();
            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (notify.NotifyType == NotifyType.ChangeControl)
                    {
                        ModuleEnv.GlobalTempParam = notify.ControlParam;
                        if (notify.ControlType == 1) ComponentControl = Module.IocModule.Resolve<IndexView>();
                        if (notify.ControlType == 2)
                        {
                            var Reader = Module.IocModule.Resolve<ReaderView>();
                            Reader.Height = ComponentControl.Height;
                            Reader.Width = ComponentControl.Width;
                            ComponentControl = Reader;
                        }
                    }
                });
            });
        }

        #region Property
        private Control _ComponentControl;
        public Control ComponentControl
        {
            get => _ComponentControl;
            set => SetAndNotify(ref _ComponentControl, value);
        }
        #endregion
    }
}
