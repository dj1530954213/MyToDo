using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MyToDo.Extensions
{
    public class PassWordExtensions
    {
        //通过依赖对象获取密码
        public static string GetPassWord(DependencyObject obj)
        {
            return (string)obj.GetValue(PassWordProperty);
        }
        //通过依赖对象设置密码
        public static void SetPassWord(DependencyObject obj, string value)
        {
            obj.SetValue(PassWordProperty, value);
        }
        //将PassWord进行注册
        public static readonly DependencyProperty PassWordProperty = DependencyProperty.RegisterAttached("PassWord",typeof(string),typeof(PassWordExtensions),new FrameworkPropertyMetadata(string.Empty,OnPassWordProppertyChanged));

        private static void OnPassWordProppertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passWordBox = d as PasswordBox;
            string passWord = (string)e.NewValue;
            //如果当前的内部值与界面中的值不一致
            if (passWordBox != null && passWordBox.Password != passWord)
            {
                passWordBox.Password = passWord;
            }
        }
    }
    /// <summary>
    /// 行为类，附加至PasswordBox这个类中，通过监听PasswordChanged来实时修改PassWord的值
    /// </summary>
    public class PassWordBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            string password = PassWordExtensions.GetPassWord(passwordBox);

            if (passwordBox != null && passwordBox.Password != password)
            {
                PassWordExtensions.SetPassWord(passwordBox, passwordBox.Password);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }
    }
}
