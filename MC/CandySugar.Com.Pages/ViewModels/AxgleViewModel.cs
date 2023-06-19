using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.Extends;
using CandySugar.Com.Pages.Views.AxgleViews;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Sdk.Component.Vip.Axgle.sdk;
using Sdk.Component.Vip.Axgle.sdk.ViewModel;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Request;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public class AxgleViewModel : BaseVMModule
    {
        public AxgleViewModel(string QueryKey, BaseVMService baseServices) : base(baseServices)
        {
            //Keyword = QueryKey;
            //if (!QueryKey.IsNullOrEmpty())
            //    Nav.NavigateAsync(new Uri(nameof(AxgleInfo), UriKind.Relative), new NavigationParameters { { "Param", Keyword } });
            //else
            //    OnAllInit();

            Nav.NavigateAsync(new Uri("AxglePlay", UriKind.Relative), new NavigationParameters { { "Param", "https://avgle.com/video/rYqcFctQzcw/%5bFHD%5d+MIDV-400+%e4%b8%89%e4%b8%8a%e6%82%a0%e4%ba%9e%c3%97MOODYZ+%e4%b8%80%e6%ac%a1%e6%80%a7%e9%99%90%e5%ae%9a%e5%a4%a7%e5%be%a9%e6%b4%bb%ef%bc%81%ef%bc%9f%e4%b8%80%e5%80%8b%e4%ba%ba%e6%80%a7%e6%84%9b%e6%8a%bd%e6%8f%92%e5%b7%b4%e5%a3%ab%e6%97%85%e9%81%8a2023+%e5%8d%b3%e5%b0%87%e5%bc%95%e9%80%80!+%e6%9c%80%e5%be%8c%e7%9a%84%e5%a4%a7%e6%84%9f%e8%ac%9d%e7%89%b9%e5%88%a5%e7%af%80%e7%9b%ae+%e4%b8%ad%e6%96%87%e5%ad%97%e5%b9%95" } });
        }

        #region Field
        private string Keyword;
        #endregion

        #region Property
        private ObservableCollection<AxgleInitResult> _Init;
        public ObservableCollection<AxgleInitResult> Init
        {
            get => _Init;
            set => SetProperty(ref _Init, value);
        }
        #endregion

        #region Command
        public RelayCommand<AxgleInitResult> ListCommand => new(element => Nav.NavigateAsync(new Uri(nameof(AxgleInfo), UriKind.Relative), new NavigationParameters { { "Param", element } }));
        #endregion

        #region ExternalMethod
        private void OnAllInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AxgleFactory.Axgle(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            AxgleType = AxgleEnum.Init,
                        };
                    }).RunsAsync()).InitResults;
                    Init = new ObservableCollection<AxgleInitResult>(result);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }
        #endregion

    }
}
