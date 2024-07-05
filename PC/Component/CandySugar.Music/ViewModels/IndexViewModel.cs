namespace CandySugar.Music.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {

        public IndexViewModel()
        {
            Title = ["单曲", "歌单", "收藏"];
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                MarginThickness = new Thickness(0, 0, 15, 70);
            else
                MarginThickness = new Thickness(0, 0, 15, 15);
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        #endregion

        #region 命令
        [RelayCommand]
        public void Changed(object item)
        { 
        
        }
        #endregion
    }
}
