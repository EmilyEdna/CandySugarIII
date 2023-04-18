using CandySugar.Com.Library;
using CandySugar.Com.Options.ComponentObject;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace CandySugar.EntryUI.ViewModels
{
    public class OptionViewModel : PropertyChangedBase
    {

        public OptionViewModel()
        {
            Route = ComponentBinding.OptionObjectModels.BackgroudLocation;
            Interval = ComponentBinding.OptionObjectModels.Interval;
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
                Interval = Interval
            };
            var path = Path.Combine(CommonHelper.OptionPath, "SystemOption.json");
            SyncStatic.WriteFile(Encoding.Default.GetBytes(new { Option= Model }.ToJson()), path);
            ComponentBinding.ForceRefresh = true;
            window.Close();
        }
        #endregion
    }
}
