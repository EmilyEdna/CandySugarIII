﻿using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Animes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Cart.sdk;
using Sdk.Component.Cart.sdk.ViewModel;
using Sdk.Component.Cart.sdk.ViewModel.Enums;
using Sdk.Component.Cart.sdk.ViewModel.Request;
using Sdk.Component.Cart.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;
using Microsoft.Maui.Dispatching;
using Application = Microsoft.Maui.Controls.Application;
using Microsoft.Maui.Controls;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class AnimeViewModel : ObservableObject
    {
        public AnimeViewModel()
        {
            Application.Current.Dispatcher.DispatchAsync(InitAsync);
            Bar = new ObservableCollection<BarModel> { 
             new BarModel{ Route="",Name="ALL" }
            };
            for (int Index = 0; Index <= 10; Index++)
            {
                var Year = (DateTime.Now.Year - Index).AsString();
                Bar.Add(new BarModel
                {
                    Name = Year,
                    Route= Year
                });
            }

        }
        #region Field
        private int Page = 1;
        private int Total;
        private int QPage = 1;
        private int QTotal;
        private string Year=string.Empty;
        #endregion

        #region Property
        [ObservableProperty]
        private string _QueryKey;
        [ObservableProperty]
        private ObservableCollection<CartInitElementResult> _InitResult;
        [ObservableProperty]
        private ObservableCollection<BarModel> _Bar;
        #endregion

        #region Method
        private async void InitAsync()
        {
            try
            {
                var result = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        CartType = CartEnum.Init,
                        CacheSpan = 5,
                        Init = new CartInit
                        {
                            Year=Year,
                            Page = Page,
                        }
                    };
                }).RunsAsync()).InitResult;
                if (Page == 1)
                {
                    Total = result.Total;
                    InitResult = new ObservableCollection<CartInitElementResult>(result.ElementResults);
                }
                else result.ElementResults.ForEach(InitResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void SearchAsync()
        {
            try
            {
                var result = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        CartType = CartEnum.Search,
                        CacheSpan = 5,
                        Search = new CartSearch
                        {
                            Page = QPage,
                            Keyword = QueryKey
                        }
                    };
                }).RunsAsync()).SearchResult;
                var TargetModel = result.ElementResults.ToMapest<List<CartInitElementResult>>();
                if (QPage == 1)
                {
                    QTotal = result.Total;
                    InitResult = new ObservableCollection<CartInitElementResult>(TargetModel);
                }
                else TargetModel.ForEach(InitResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Next(CartInitElementResult Model)
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(CollectView)], new Dictionary<string, object> { { "Param", Model } });
        }
        #endregion

        #region  Command
        public RelayCommand<CartInitElementResult> CollectCommand => new(Next);
        public RelayCommand QueryCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty()) return;
            QPage = 1;
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        });
        public RelayCommand MoreCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty())
            {
                Page += 1;
                if (Page <= Total)
                    Application.Current.Dispatcher.DispatchAsync(InitAsync);
            }
            else
            {
                QPage += 1;
                if (QPage <= QTotal)
                    Application.Current.Dispatcher.DispatchAsync(SearchAsync);
            }
        });
        public RelayCommand<string> CatalogCommand => new(obj => {
            Year = obj;
            Page = 1;
            Application.Current.Dispatcher.DispatchAsync(InitAsync);
        });
        #endregion
    }
}
