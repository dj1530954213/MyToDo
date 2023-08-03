using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace MyToDo.ViewModels
{
    class MainViewModel: BindableBase,IConfigureService
    {
        public DelegateCommand minimize { get; private set; }//最小化窗口
        public DelegateCommand maximize { get; private set; }//最大化窗口
        public DelegateCommand closeWindow { get; private set; }//关闭窗口
        public DelegateCommand<MenuBar> navigateCommand { get; private set; }//菜单导航
        public DelegateCommand forwordCommand { get; private set; }//前进
        public DelegateCommand backCommand { get; private set; }//后退
        public DelegateCommand LogOutCommand { get; private set; }//注销当前用户

        public MainViewModel(IRegionManager regionManager, IDialogHostService dialogHostService,IContainerProvider containerProvider)//继承IRegionManager来操作指定的区域
        {
            #region 响应方法实例化
            minimize = new DelegateCommand(MinimzeWindow);
            maximize = new DelegateCommand(MaximizeWindow);
            closeWindow = new DelegateCommand(CloseWindow);
            navigateCommand = new DelegateCommand<MenuBar>(Navigate);
            LogOutCommand = new DelegateCommand(() =>
            {
                //注销当前用户,此方法是申明在App中的一个静态的方法
                App.LogOut(_containerProvider);
            });
            //实现前进处理命令实例化以及匿名委托
            forwordCommand = new DelegateCommand(() =>
            {
                if (this.journal!=null && this.journal.CanGoForward)
                {
                    this.journal.GoForward();
                }
            });
            //实现后退处理命令实例化以及匿名委托
            backCommand = new DelegateCommand(() =>
            {
                if (this.journal != null && this.journal.CanGoBack)
                {
                    this.journal.GoBack();
                }
            });
            #endregion

            #region 属性实例化
            MenuBars = new ObservableCollection<MenuBar>();
            #endregion

            #region 私有字段实例化
            this.regionManager = regionManager;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            #endregion

        }
        /// <summary>
        /// 注册区域导航以及前进后退日志
        /// </summary>
        /// <param name="obj"></param>
        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.NameSpace)) return;
            try
            {
                regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, back =>
                {
                    this.journal = back.Context.NavigationService.Journal;//获取日志
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private readonly IRegionManager regionManager;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;
        private  IRegionNavigationJournal journal;
        private ObservableCollection<MenuBar> _menuBars;
        public ObservableCollection<MenuBar> MenuBars
        {
            get => _menuBars;
            set
            {
                _menuBars = value;
                this.RaisePropertyChanged(nameof(MenuBars));
            }
        }
        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                this.RaisePropertyChanged(nameof(UserName));
            }
        }

        private async void CloseWindow()
        {
            var dialogReuslt = await _dialogHostService.Question("温馨提示", "确认关闭软件？");
            if (dialogReuslt.Result != ButtonResult.OK)
            {
                return;
            }
            Application.Current.MainWindow.Close();
        }

        private void MaximizeWindow()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void MinimzeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView"});
            MenuBars.Add(new MenuBar() { Icon = "Notebook", Title = "待办事项", NameSpace = "ToDoView" });
            MenuBars.Add(new MenuBar() { Icon = "NotePlus", Title = "备忘录", NameSpace = "MemoView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingView" });
        }
        /// <summary>
        /// 配置首页初始化相关的额操作
        /// </summary>
        public void Configure()
        {
            UserName = AppSession.UserName;
            CreateMenuBar();
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }
    }
}
