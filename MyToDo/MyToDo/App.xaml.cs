using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DryIoc;
using MyToDo.Common;
using MyToDo.Service;
using MyToDo.ViewModels;
using MyToDo.ViewModels.Dialogs;
using MyToDo.Views;
using MyToDo.Views.Dialogs;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;

namespace MyToDo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App:PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            /*注册WebAPI接口*/
            containerRegistry.GetContainer()
                .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.GetContainer().RegisterInstance(@"http://localhost:17408/", serviceKey: "webUrl");
            containerRegistry.Register<IToDoService, ToDoService>();
            containerRegistry.Register<IMemoService, MemoService>();
            containerRegistry.Register<ILoginService, LoginService>();

            /* 主页面模块注册*/
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<ToDoView, ToDoViewModel>();
            containerRegistry.RegisterForNavigation<MemoView, MemoViewModel>();

            /*设置页面模块注册*/
            containerRegistry.RegisterForNavigation<SkinView,SkinViewModel>();
            containerRegistry.RegisterForNavigation<AboutView>();

            /*弹窗注册*/
            containerRegistry.RegisterForNavigation<AddToDoView, AddToDoViewModel>();
            containerRegistry.RegisterForNavigation<AddMemoView, AddMemoViewModel>();
            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
            containerRegistry.Register<IDialogHostService, DialogHostService>();


        }
        /// <summary>
        /// 重写prism中的初始化方法
        /// </summary>
        protected override void OnInitialized()
        {
            //登录界面弹窗
            var loginDialog = Container.Resolve<IDialogService>();
            loginDialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }
                //首先先获取在mainwindow中定义的首页初始化处理方法service,通过断点观察可以发现，传入的DataContext是mainviewmodule，且其继承于IConfigureService，所以我们可以得到
                //IConfigureService的实例自然可以调用Configure这个方法
                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                {
                    service.Configure();
                }
                base.OnInitialized();
            });
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        public static void LogOut(IContainerProvider containerProvider)
        {
            Current.MainWindow.Hide();
            //登录界面弹窗
            var loginDialog = containerProvider.Resolve<IDialogService>();
            loginDialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }
                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                {
                    service.Configure();
                }
                Current.MainWindow.Show();
            });
        }
    }
}
