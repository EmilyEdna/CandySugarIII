using CandySugar.Com.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Axgle.ViewModels
{
    public partial class Index5ViewModel : BasicObservableObject
    {
        private IService<AxgleModel> Service;
        public Index5ViewModel()
        {
            Service = IocDependency.Resolve<IService<AxgleModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            CollectResult = new(Service.QueryAll());
        }

        #region 属性
        [ObservableProperty]
        private ObservableCollection<AxgleModel> _CollectResult;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                BorderWidth -= 60;
            }
        }
        #endregion

        #region 命令
        [RelayCommand]
        public void Remove(Guid id)
        {
            Service.Remove(id);
            CollectResult = new(Service.QueryAll());
        }
        [RelayCommand]
        public void Play(AxgleModel element)
        {
            if (element.Platfrom == "A24" || element.Platfrom == "JAXX")
                Application.Current.Dispatcher.Invoke(() => new CandyWebPlayControl(element.Route, false, true).Show());
            else
                OnDetail(element);
        }
        #endregion

        #region 方法
        private void OnDetail(AxgleModel input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MissFactory.Miss(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            FuncType = FuncEnum.Detail,
                            PlatformType = Enum.Parse<PlatformEnum>(input.Platfrom),
                            Play = new MissPlay
                            {
                                Route = input.Route
                            }
                        };
                    }).RunsAsync()).PlayResult.Play;

                    Application.Current.Dispatcher.Invoke(() => new CandyWebPlayControl(result, true, false).Show());
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    CandyNotify.Error(CommonHelper.ComponentErrorInformation);
                }
            });
            #endregion
        }
    }
}
