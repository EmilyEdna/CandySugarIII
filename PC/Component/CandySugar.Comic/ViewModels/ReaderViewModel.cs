﻿using CandyControls;
using System.Collections.Generic;

namespace CandySugar.Comic.ViewModels
{
    public partial class ReaderViewModel : BasicObservableObject
    {
        public ReaderViewModel()
        {
            Picture = [];
            ((List<string>)Module.Param)?.ForEnumerEach((item, index) =>
            {
                Picture.Add(new WatchInfo
                {
                    Index = index,
                    Route = item,
                });
            });
            Current = Picture.FirstOrDefault();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 字段
        public ReaderView Views;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                BorderHeight = 1400;
                BorderWidth = 1200;
                MarginThickness = new Thickness(0, 0, 20, 55);
            }
            else
            {
                BorderHeight = 1200;
                BorderWidth = 1000;
                MarginThickness = new Thickness(0, 0, 10, 0);
            }
            if (Picture != null)
                Picture = new(Picture.ToList());
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<WatchInfo> _Picture;
        [ObservableProperty]
        private WatchInfo _Current;
        #endregion

        #region 命令
        [RelayCommand]
        public void Handle(string input)
        {
            var Data = input.AsInt();
            if (Data == -1)
            {
                if (Current.Index + Data < 0) return;
                Current = Picture.ElementAtOrDefault(Current.Index + Data);
            }
            else if (Data == 1)
            {
                if (Current.Index + Data >= Picture.Count) return;
                Current = Picture.ElementAtOrDefault(Current.Index + Data);
            }
            else
                ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(false);
        }
        #endregion
    }
}
