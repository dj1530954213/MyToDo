using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Shared.Dtos;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyToDo.ViewModels.Dialogs
{
    public class AddMemoViewModel :BindableBase, IDialogHostAware
    {
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public string DialogHostName { get; set; }
        private MemoDto _memoForAdd;
        public MemoDto MemoForAdd
        {
            get => _memoForAdd;
            set
            {
                _memoForAdd = value;
                this.RaisePropertyChanged(nameof(MemoForAdd));
            }
        }
        public AddMemoViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }
        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName,new DialogResult(ButtonResult.Cancel));
            }
        }
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(MemoForAdd.Title) || string.IsNullOrWhiteSpace(MemoForAdd.Content))
            {
                return;
            }
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters parameters = new DialogParameters();
                //将ToDoForAdd与界面中实现绑定，并添加到窗口的返回参数内
                parameters.Add("Value", MemoForAdd);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }
        public void OnDialogOpend(IDialogParameters dialogParameters)
        {
            //判断当窗体打开的时候是否为新增
            if (dialogParameters.ContainsKey("Value"))
            {
                MemoForAdd = dialogParameters.GetValue<MemoDto>("Value");
            }
            else
            {
                MemoForAdd = new MemoDto();
            }
        }
    }
}
