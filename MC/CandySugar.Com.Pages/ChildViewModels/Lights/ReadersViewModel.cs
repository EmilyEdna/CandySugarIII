using CandySugar.Com.Library;
using CommunityToolkit.Mvvm.ComponentModel;
using NPOI.SS.Formula.Functions;
using Sdk.Component.Lovel.sdk;
using Sdk.Component.Lovel.sdk.ViewModel;
using Sdk.Component.Lovel.sdk.ViewModel.Enums;
using Sdk.Component.Lovel.sdk.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Pages.ChildViewModels.Lights
{
    public partial class ReadersViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Route = query["Route"].ToString();
            ReaderAsync();
        }
        #region Field
        private string Route;
        #endregion

        #region Property
        [ObservableProperty]
        private ObservableCollection<string> _Words;
        #endregion

        #region Method
        private async void ReaderAsync()
        {
            try
            {
                var result = (await LovelFactory.Lovel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan =5,
                        LovelType = LovelEnum.Content,
                        Content = new LovelContent
                        {
                            ChapterRoute = Route
                        }
                    };
                }).RunsAsync()).ContentResult;
                if (result.Content != null || result.Image != null)
                {
                    if (result.Content != null)
                    {
                        if (result.Content.Equals("因版权问题，文库不再提供该小说的阅读！"))
                        {
                            "因版权问题，请前往下载!".Info();
                            return;
                        }
                        Words = new ObservableCollection<string>(result.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        #endregion
    }
}
