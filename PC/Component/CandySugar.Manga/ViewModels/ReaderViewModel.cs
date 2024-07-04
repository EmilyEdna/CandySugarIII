namespace CandySugar.Manga.ViewModels
{
    public partial class ReaderViewModel : BasicObservableObject
    {
        public ReaderViewModel()
        {
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            Oninit();
        }

        #region 字段
        public ReaderView Views;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                BorderHeight = 1600;
                BorderWidth = SystemParameters.FullPrimaryScreenWidth;
                MarginThickness = new Thickness(0, 0, 20, 55);
            }
            else
            {
                BorderHeight = 1200;
                BorderWidth = 1000;
                MarginThickness = new Thickness(0, 0, 10, 0);
            }
            Picture = new(Picture.ToList());
        }
        #endregion

        #region 属性          
        [ObservableProperty]
        private ObservableCollection<string> _Picture;
        #endregion

        #region 方法
        private void Oninit()
        {
            var imgBytes = Module.Param as List<byte[]>;
            Picture = new(imgBytes.Select(Convert.ToBase64String));
        }
        #endregion

        #region 命令
        [RelayCommand]
        public void Handle(string input)
        {
            ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(false);
        }
        #endregion
    }
}
