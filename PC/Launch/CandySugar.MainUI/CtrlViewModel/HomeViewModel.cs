using CandySugar.Com.Data;
using CandySugar.Com.Data.Entity.NovelEntity;
using CandySugar.Com.Options.ComponentGeneric;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.MainUI.CtrlViewModel
{
    internal partial class HomeViewModel : ObservableObject
    {
        private IService<NovelModel> Service;
        public HomeViewModel()
        {
            Service = IocDependency.Resolve<IService<NovelModel>>();
            NovelData = new(Service.QueryAll());
        }

        #region 属性
        [ObservableProperty]
        private ObservableCollection<NovelModel> _NovelData;
        #endregion

        #region 命令
        [RelayCommand]
        public void Novel(object input)
        {
            if (input != null)
            {
                if ((input as TabItem).Header.AsString().Equals("小说"))
                {
                    NovelData = new(Service.QueryAll());
                }
            }
        }
        [RelayCommand]
        public void RemoveNovel(Guid Id) => Service.Remove(Id);
        [RelayCommand]
        public void Keep(NovelModel novel) => GenericDelegate.ChangeContentAction?.Invoke(new
        {
            novel.Platform,
            MD5= novel.Detail.ToMd5(),
            Index = novel.Current,
            Current = novel.Route,
            Chapters = Encoding.UTF8.GetString(Convert.FromBase64String(novel.Detail)).ToModel<JArray>().Select(t=>new {
                Route = t["Route"].AsString(),
                Chapter = t["Chapter"].AsString()
            }).ToList()
        });
        #endregion

    }
}
