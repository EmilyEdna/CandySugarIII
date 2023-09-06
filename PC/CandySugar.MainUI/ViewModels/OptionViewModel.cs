using CandySugar.Com.Library;
using CandySugar.Com.Options.ComponentObject;
using Stylet;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace CandySugar.MainUI.ViewModels
{
    public class OptionViewModel : PropertyChangedBase
    {

        public OptionViewModel()
        {
            Route = ComponentBinding.OptionObjectModels.BackgroudLocation;
            Interval = ComponentBinding.OptionObjectModels.Interval;
            UseProxy = ComponentBinding.OptionObjectModels.UseProxy;
        }

        #region Property
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }
        private double _Interval;
        public double Interval
        {
            get => _Interval;
            set => SetAndNotify(ref _Interval, value);
        }
        private bool _UseProxy;
        public bool UseProxy
        {
            get => _UseProxy;
            set
            {
                SetAndNotify(ref _UseProxy, value);
                ProxyState = value ? "启用" : "禁用";
            }
                    
        }
        private string _ProxyState;
        public string ProxyState
        {
            get => _ProxyState;
            set => SetAndNotify(ref _ProxyState, value);
        }
        #endregion

        #region Command
        public void FolderCommand()
        {
            FolderBrowserDialog dialog = new()
            {
                RootFolder = Environment.SpecialFolder.Desktop
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Route = dialog.SelectedPath;
            }
        }
        public void CloseCommand(Window window)
        {
            if (Route.IsNullOrEmpty())
            {
                window.Close();
                return;
            }
            OptionObjectModel Model = new OptionObjectModel
            {
                Cache = 5,
                BackgroudLocation = Route,
                Interval = Interval,
                Raw = ComponentBinding.OptionObjectModels.Raw,
                UseProxy = UseProxy
            };
            var path = Path.Combine(CommonHelper.OptionPath, "SystemOption.json");
            SyncStatic.WriteFile(Encoding.Default.GetBytes(new { Option = Model }.ToJson()), path);
            ComponentBinding.ForceRefresh = true;
            window.Close();
        }
        #endregion
    }
}
