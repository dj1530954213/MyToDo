using DryIoc;
using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyToDo.ViewModels
{
    public class MsgViewModel : BindableBase, IDialogHostAware
    {
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public string DialogHostName { get; set; } = "Root";
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                this.RaisePropertyChanged(nameof(Title));
            }
        }
        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                this.RaisePropertyChanged(nameof(Content));
            }
        }

        public MsgViewModel()
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
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters parameters = new DialogParameters();
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }

        public void OnDialogOpend(IDialogParameters dialogParameters)
        {
            if (dialogParameters.ContainsKey("Title"))
            {
                Title = dialogParameters.GetValue<string>("Title");
            }
            if (dialogParameters.ContainsKey("Content"))
            {
                Content = dialogParameters.GetValue<string>("Content");
            }
        }
    }
}
