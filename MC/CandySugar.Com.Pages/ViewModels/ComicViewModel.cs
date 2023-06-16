using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.Extends;
using CandySugar.Com.Pages.Views.ComicViews;
using NPOI.SS.Formula.Functions;
using Sdk.Component.Vip.Comic.sdk;
using Sdk.Component.Vip.Comic.sdk.ViewModel;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Request;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public class ComicViewModel : BaseVMModule
    {
        public ComicViewModel(string QueryKey, BaseVMService baseServices) : base(baseServices)
        {
            Keyword = QueryKey;
            OnComicInit();
        }

        public override void OnAppearing()
        {
            PageIndex = 1;
        }

        #region Field
        private int PageIndex;
        private string Keyword;
        #endregion

        #region Property
        private ObservableCollection<SearchElementResult> _SearchResult;
        public ObservableCollection<SearchElementResult> SearchResult
        {
            get => _SearchResult;
            set => SetProperty(ref _SearchResult, value);
        }
        #endregion

        #region ExternalMethod
        private void OnComicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            ComicType = ComicEnum.Search,
                            Search = new ComicSearch
                            {
                                Keyword = Keyword,
                                Page = 1
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Total = result.Total;
                    SearchResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }
        #endregion

        #region ExternalMoreMethod
        private void OnLoadMoreComicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            ComicType = ComicEnum.Search,
                            Search = new ComicSearch
                            {
                                Keyword = Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    result.Results.ForEach(SearchResult.Add);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }
        #endregion

        #region Command
        public DelegateCommand LoadCommand => new(LoadMethod);

        public DelegateCommand<SearchElementResult> PreivewCommand => new(element=>Nav.NavigateAsync(new Uri(nameof(ComicInfo),UriKind.Relative),new NavigationParameters { { "Param",element } }));
        #endregion

        #region Method
        private void LoadMethod() 
        {
            if (PageIndex > Total) return;
                OnLoadMoreComicInit();
        }
        #endregion
    }
}
