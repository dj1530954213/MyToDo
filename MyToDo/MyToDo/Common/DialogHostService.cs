using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using Prism.Ioc;
using MaterialDesignThemes.Wpf;
using Prism.Mvvm;
using System.Windows;

namespace MyToDo.Common
{
    /// <summary>
    /// 对话主机服务
    /// </summary>
    public class DialogHostService : DialogService,IDialogHostService
    {
        private readonly IContainerExtension _containerExtension;

        public DialogHostService(IContainerExtension containerExtension) : base(containerExtension)
        {
            _containerExtension = containerExtension;
        }

        public async Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string dialogHostName = "Root")
        {
            if (parameters == null)
                parameters = new DialogParameters();

            //从容器当中获得弹出窗口的实例,所以自定义的弹窗就需要注册至容器中才能在这进行获取
            var content = _containerExtension.Resolve<object>(name);

            //验证实例的有效性 ,这里使用了模式匹配的特性
            /*
             * 这段代码使用了 C# 7 中的新特性，称为模式匹配。当content不是FrameworkElement或其子类类型的时候抛出异常，如果是的话则复制给dialogContent
             */
            if (!(content is FrameworkElement dialogContent))
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
                ViewModelLocator.SetAutoWireViewModel(view, true);

            if (!(dialogContent.DataContext is IDialogHostAware viewModel))
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

            viewModel.DialogHostName = dialogHostName;

            DialogOpenedEventHandler eventHandler = (sender, eventArgs) =>
            {
                if (viewModel is IDialogHostAware aware)
                {
                    aware.OnDialogOpend(parameters);
                }
                eventArgs.Session.UpdateContent(content);
            };

            return (IDialogResult) await DialogHost.Show(dialogContent, viewModel.DialogHostName, eventHandler);
        }
    }
}
