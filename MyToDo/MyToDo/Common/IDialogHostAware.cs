using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace MyToDo.Common
{
    public interface IDialogHostAware
    {

        public DelegateCommand SaveCommand { get;  set; }

        public DelegateCommand CancelCommand { get; set; }

        public string DialogHostName { get; set; }

        void OnDialogOpend(IDialogParameters dialogParameters);
        
    }
}
