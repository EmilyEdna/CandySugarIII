using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Axgles;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Miss.Sdk;
using Sdk.Component.Vip.Miss.Sdk.ViewModel;
using Sdk.Component.Vip.Miss.Sdk.ViewModel.Enums;
using Sdk.Component.Vip.Miss.Sdk.ViewModel.Request;
using Sdk.Component.Vip.Miss.Sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Axgles
{
    public partial class LinkViewModel : ObservableObject, IQueryAttributable
    {
        private string Cover;
        private PlatformEnum Platform;
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var Temp = (Dictionary<string, object>)query["Param"];
            Title = Temp["Title"].ToString();
            Cover = Temp["Cover"].ToString();
            Platform = (PlatformEnum)Temp["Platform"];
            Routes = [];
            ((Dictionary<string, string>)Temp["Result"]).ForDicEach((k, v) =>
            {

                Routes.Add(new Model
                {
                    Key = k,
                    Value = v
                });

            });
            Links = [.. (List<MissRelatedElementResult>)Temp["Links"]];
        }

        #region Title
        [ObservableProperty]
        private string _Title;
        [ObservableProperty]
        private ObservableCollection<Model> _Routes;
        [ObservableProperty]
        private ObservableCollection<MissRelatedElementResult> _Links;
        #endregion

        #region Command
        [RelayCommand]
        public void Play(string input) => Next(input);

        public RelayCommand<MissRelatedElementResult> ViewCommand => new(PlayAsync);

        [RelayCommand]
        public void Collect() => Insert();
        #endregion

        #region Method

        private async void PlayAsync(MissRelatedElementResult input)
        {
            var option = await IocDependency.Resolve<ICandyService>().GetOption();
            try
            {
                var Keys= option.DecodeDataKey.Split(",");
                string Key = string.Empty;
                if (Platform == PlatformEnum.A24) Key = Keys.First();
                if (Platform == PlatformEnum.JAXX) Key = Keys.Last();

                var result = (await MissFactory.Miss(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        FuncType = FuncEnum.Detail,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Play = new MissPlay
                        {
                            Route = input.Route,
                            DecodeKey= Key,
                            DecodeM3u8Key=option.DecodePlayKey
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
                    Category = 2,
                    Cover = Cover,
                    Name = Title+$"-{i+1}",
                    Route = Routes[i].Value
                });
            }
        }

        private async void Next(string input)
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Param", input }, { "Is24Net", true } });
        }
        #endregion
    }

    public class Model
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
