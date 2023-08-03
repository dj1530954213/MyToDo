using Prism.Events;
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
using MaterialDesignThemes.Wpf;
using MyToDo.Extensions;
using Window = System.Windows.Window;

namespace MyToDo.Views.Dialogs
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        private bool isDragging;
        private Point clickPosition;
        private Window parentWindow;

        public LoginView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            //获取父级窗机对象用于控件窗体移动
            parentWindow = Window.GetWindow(this);
            //通过扩展方法注册消息通知事件
            eventAggregator.ResgiterMessage(arg =>
            {
                LoginSnackbar.MessageQueue.Enqueue(arg.Message);
            },"Login");

            this.MouseLeftButtonDown += UserControl_MouseLeftButtonDown;
            this.MouseMove += UserControl_MouseMove;
            this.MouseLeftButtonUp += UserControl_MouseLeftButtonUp;
        }
        #region 用户控件窗体移动逻辑：通过窗体初始化获取此父级窗体的实例，并将窗体的鼠标点击移动事件进行逻辑处理

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(this);
            CaptureMouse();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;
                var parentWindow = Window.GetWindow(this);

                if (parentWindow != null)
                {
                    parentWindow.Left += offsetX;
                    parentWindow.Top += offsetY;
                }
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ReleaseMouseCapture();
        }

        #endregion



    }
}
