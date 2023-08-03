using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Shared.Dtos;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyToDo.ViewModels.Dialogs
{
    public class AddToDoViewModel : BindableBase,IDialogHostAware
    {

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public string DialogHostName { get; set; }
        private ToDoDto _toDoForAdd;
        public ToDoDto ToDoForAdd
        {
            get => _toDoForAdd;
            set
            {
                _toDoForAdd = value;
                this.RaisePropertyChanged(nameof(ToDoForAdd));
            }
        }
        public AddToDoViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }
        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
            }
        }
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(ToDoForAdd.Title) || string.IsNullOrWhiteSpace(ToDoForAdd.Content))
            {
                return;
            }
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters parameters = new DialogParameters();
                //将ToDoForAdd与界面中实现绑定，并添加到窗口的返回参数内
                parameters.Add("Value",ToDoForAdd);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }
        public void OnDialogOpend(IDialogParameters dialogParameters)
        {
            //判断当窗体打开的时候是否为新增,如果包含Value则为编辑否则新建一个新的实例
            if (dialogParameters.ContainsKey("Value"))
            {
                ToDoForAdd = dialogParameters.GetValue<ToDoDto>("Value");
            }
            else
            {
                ToDoForAdd = new ToDoDto();
            }
        }
    }
}
