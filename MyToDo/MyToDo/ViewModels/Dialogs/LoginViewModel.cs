using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyToDo.Common;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyToDo.ViewModels.Dialogs
{
    public class LoginViewModel:BindableBase,IDialogAware
    {
        private readonly ILoginService _loginService;
        private readonly IEventAggregator _aggregator;
        private string _account;
        public string Account
        {
            get => _account;
            set
            {
                _account = value;
                this.RaisePropertyChanged(nameof(Account));
            }
        }

        private string _passWord;
        public string PassWord
        {
            get => _passWord;
            set
            {
                _passWord = value;
                this.RaisePropertyChanged(nameof(PassWord));
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                this.RaisePropertyChanged(nameof(SelectedIndex));
            }
        }

        private RegisterUserDto _registerUserDto;
        public RegisterUserDto RegisterUserDto
        {
            get => _registerUserDto;
            set
            {
                _registerUserDto = value;
                this.RaisePropertyChanged(nameof(RegisterUserDto));
            }
        }
        
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public LoginViewModel(ILoginService loginService,IEventAggregator aggregator)
        {
            _registerUserDto = new RegisterUserDto();
            _aggregator = aggregator;
            _loginService = loginService;
            ExecuteCommand = new DelegateCommand<string>(Execute);
        }
        private void Execute(string obj)
        {
            switch (obj)
            {
                case "Login":  Login();//登录
                    break;
                case "Logout": Logout();//取消登录
                    break;
                case "Go"://跳转至注册页面
                    SelectedIndex = 1;
                    break;
                case "Return"://返回登录页面
                    SelectedIndex = 0;
                    break;
                case "Register"://注册账号
                    Register();
                    break;

            }
        }

        private async void Register()
        {
            if (string.IsNullOrWhiteSpace(RegisterUserDto.Account)||
                string.IsNullOrWhiteSpace(RegisterUserDto.UserName)||
                string.IsNullOrWhiteSpace(RegisterUserDto.PassWord)||
                string.IsNullOrWhiteSpace(RegisterUserDto.ConfirmPassWord))
            {
                return;
            }

            if (RegisterUserDto.PassWord!=RegisterUserDto.ConfirmPassWord)
            {
                return;
            }

            var result = await _loginService.RegisterAsync(new UserDto()
            {
                Account = RegisterUserDto.Account,
                UserName = RegisterUserDto.UserName,
                PassWord = RegisterUserDto.PassWord,
            });

            if (result != null && result.Status)
            {
                _aggregator.SendMessage("注册成功，即将跳转至登录页面", "Login");
                SelectedIndex = 0;
                return;
            }
            //注册失败提示..
            _aggregator.SendMessage($"注册失败:{result.Message}", "Login");
        }

        private void Logout()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));
        }

        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(PassWord))
            {
                _aggregator.SendMessage("账号密码不得为空，请检查后重新输入","Login");
                return;
            }
            var loginResult = await _loginService.LoginAsync(new UserDto()
            {
                Account = _account, PassWord = _passWord
            });
            if (loginResult.Status)
            {
                AppSession.UserName = loginResult.Result.UserName;
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            else
            {
                _aggregator.SendMessage("登录失败请检查账号及密码后重新登录", "Login");
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        public string Title { get; }
        public event Action<IDialogResult>? RequestClose;
    }
}
