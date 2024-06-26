using Sdk.Component.Vip.Wall.sdk.ViewModel.Enums;

namespace CandySugar.WallPaper.ViewModels
{
    public class IndexViewModel: PropertyChangedBase
    {
        private IService<WallModel> Service;
        public IndexViewModel()
        {
            Service = IocDependency.Resolve<IService<WallModel>>();
            Platform = PlatformEnum.Wallhaven;
           Title = ["常规", "一般", "可疑", "收藏"];
        }

        #region Field
        private PlatformEnum Platform;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }
        #endregion

        #region Method
        #endregion

        #region Command
        public RelayCommand<object> ChangedCommand => new((item) => {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();

              /*  if (Index == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
                if (Index == 2)
                {
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
                if (Index == 3)
                {
                    View.ActiveAnime = 4;
                    View.AnimeX4.Begin();
                }*/
            }

        });
        #endregion
    }
}
