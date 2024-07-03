namespace CandySugar.Manga.ViewModels
{
    public partial class ReaderViewModel : ObservableObject
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
                Height = 1400;
                Width = SystemParameters.FullPrimaryScreenWidth;
                MarginThickness = new Thickness(0, 0, 20, 55);
            }
            else
            {
                Height = 1200;
                Width = 1000;
                MarginThickness = new Thickness(0, 0, 10, 0);
            }
        }
        #endregion

        #region 属性          
        [ObservableProperty]
        private Thickness _MarginThickness;
        [ObservableProperty]
        private double _Width;
        [ObservableProperty]
        private double _Height;
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
