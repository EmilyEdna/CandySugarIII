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
            Keyword = QueryKey;
            if (!QueryKey.IsNullOrEmpty())
                Nav.NavigateAsync(new Uri(nameof(AxgleInfo), UriKind.Relative), new NavigationParameters { { "Param", Keyword } });
            else
                OnAllInit();
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
