using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyToDo.Extensions;
using Prism.Events;
using WindowState = System.Windows.WindowState;

namespace MyToDo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            //通过扩展方法注册等待窗口
            eventAggregator.Register(arg =>
            {
                //将左侧伸展栏的是否展开与UpdateModel中的属性进行关联
                DialogHost.IsOpen = arg.IsOpen;
                if (DialogHost.IsOpen)
                {
                    DialogHost.DialogContent = new ProgressView();
                }
            });
            //通过扩展方法注册消息通知事件
            eventAggregator.ResgiterMessage(arg =>
            {
                Snackbar.MessageQueue.Enqueue(arg);
            });

            //导航栏拖动   注册鼠标按下事件
            colorZone.MouseDown += (o, e) =>
            {
                //如果左键按下
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            //双击主题框放大功能
            colorZone.MouseDoubleClick += (o, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            };

            //选择页面后菜单栏自动收缩
            listBoxMenuBar.SelectionChanged += (o, e) =>
            {
                drawerHost.IsLeftDrawerOpen = false;
            };
        }
    }
}
