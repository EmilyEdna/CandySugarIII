using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Axgles;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Sdk.Component.Vip.Jron.sdk;
using Sdk.Component.Vip.Jron.sdk.ViewModel;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Request;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Response;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Axgles
{
    public partial class LinkViewModel : ObservableObject, IQueryAttributable
    {
        private string Cover;
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var Temp = (Dictionary<string, object>)query["Param"];
            Title = Temp["Title"].ToString();
            Cover = Temp["Cover"].ToString();
            Routes = [];
            ((Dictionary<string, string>)Temp["Result"]).ForDicEach((k, v) =>
            {

                Routes.Add(new Model
                {
                    Key = k,
                    Value = v
                });

            });
            Links = [.. (List<JronRelatedElementResult>)Temp["Links"]];
        }

        #region Title
        [ObservableProperty]
        private string _Title;
        [ObservableProperty]
        private ObservableCollection<Model> _Routes;
        [ObservableProperty]
        private ObservableCollection<JronRelatedElementResult> _Links;
        #endregion

        #region Command
        [RelayCommand]
        public void Play(string input) => Next(input);

        public RelayCommand<JronRelatedElementResult> ViewCommand => new(PlayAsync);

        [RelayCommand]
        public void Collect() => Insert();
        #endregion

        #region Method

        private async void PlayAsync(JronRelatedElementResult input)
        {
            try
            {
                var result = (await JronFactory.Jron(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        JronType = JronEnum.Detail,
                        PlatformType = PlatformEnum.A24,
                        CacheSpan = 5,
                        Play = new JronPlay
                        {
                            Route = input.Route
                        }
                    };
                }).RunsAsync()).PlayResult;
                Title = input.Title;
                Links = [.. result.ElementResults];
                var Params = new Dictionary<string, object>();
                Routes = [];
                result.Plays.ForDicEach((k, v) =>
                {
                    Routes.Add(new Model
                    {
                        Key = k,
                        Value = v
                    });

                });
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Insert()
        {
            for (int i = 0; i < Routes.Count; i++)
            {
                await IocDependency.Resolve<ICandyService>().Add(new CollectModel
                {
                    Category = 3,
                    Cover = Cover,
                    Name = Title+$"-{i+1}",
                    Route = Routes[i].Value
                });
            }
        }

        private async void Next(string input)
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Param", input } });
        }
        #endregion
    }

    public class Model
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
