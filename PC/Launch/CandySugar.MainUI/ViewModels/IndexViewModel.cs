using CandyControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Enums;
using CandySugar.MainUI.Views;
using CommunityToolkit.Mvvm.Input;
using Stylet;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CandySugar.MainUI.ViewModels
{
    public class IndexViewModel : Conductor<IScreen>
    {
        public IndexViewModel()
        {
            this.Title = $"甜糖V{Assembly.GetExecutingAssembly().GetName().Version}";

        }

        protected override void OnActivate()
        {
            SearchHistory = ["1", "2"];
            CreateMenuUI();
        }


        #region 属性
        private string _Title;
        public string Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private string _Keyword;
        public string Keyword
        {
            get => _Keyword;
            set => SetAndNotify(ref _Keyword, value);
        }

        private Control _Menus;
        public Control Mnues
        {
            get => _Menus;
            set => SetAndNotify(ref _Menus, value);
        }

        private ObservableCollection<string> _SearchHistory;
        public ObservableCollection<string> SearchHistory
        {
            get => _SearchHistory;
            set => SetAndNotify(ref _SearchHistory, value);
        }
        #endregion

        #region UI
        private void CreateMenuUI()
        {
            var Menu = new CandyMenu();
            Menu.SetResourceReference(CandyMenu.FontFamilyProperty, "FontStyle");
            var FMainItem = new CandyMenuItem
            {
                Header = "基础插件",
                CommandParameter = EHandle.None,
            };
            var SMainItem = new CandyMenuItem
            {
                Header = "会员插件",
                CommandParameter = EHandle.None,
            };
            var TMainItem = new CandyMenuItem
            {
                Header = "系统功能",
                CommandParameter = EHandle.None,
            };
            ComponentBinding.ComponentObjectModelGroups.Normal.ForEach(item =>
            {
                var SubItem = new CandyMenuItem { Header = item.Description, CommandParameter = (EHandle)item.Code };
                SubItem.SetBinding(CandyMenuItem.CommandProperty, new Binding()
                {
                    Path = new PropertyPath("ActiveCommad", (EHandle)item.Code),
                    Source = ((IndexView)View).DataContext
                });
                FMainItem.Items.Add(SubItem);
            });
            ComponentBinding.ComponentObjectModelGroups.Vip.ForEach(item =>
            {
                var SubItem = new CandyMenuItem { Header = item.Description, CommandParameter = (EHandle)item.Code };
                SubItem.SetBinding(CandyMenuItem.CommandProperty, new Binding()
                {
                    Path = new PropertyPath("ActiveCommad", (EHandle)item.Code),
                    Source = ((IndexView)View).DataContext
                });
                SMainItem.Items.Add(SubItem);
            });
            ComponentBinding.FunctionObjectModels.ForEach(item =>
            {
                var SubItem = new CandyMenuItem { Header = item.Description, CommandParameter = (EHandle)item.Code };
                SubItem.SetBinding(CandyMenuItem.CommandProperty, new Binding()
                {
                    Path = new PropertyPath("ActiveCommad", (EHandle)item.Code),
                    Source = ((IndexView)View).DataContext
                });
                TMainItem.Items.Add(SubItem);
            });
            Menu.Items.Add(FMainItem);
            Menu.Items.Add(SMainItem);
            Menu.Items.Add(TMainItem);
            Mnues = Menu;
        }
        #endregion

        #region 命令
        public RelayCommand<EHandle> ActiveCommad => new(obj =>
        {
            var x = obj;
        });

        public RelayCommand<string> AutoSelectActiveCommand => new(obj =>
        {
           
        });
        public RelayCommand<string> SearchActiveCommand => new(obj =>
        {

        });
        #endregion
    }
}
