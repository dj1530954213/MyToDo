using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyToDo.Common;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace MyToDo.ViewModels
{
    public class ToDoViewModel: NavigationViewModel
    {
        private readonly IDialogHostService _Messagedialog;
        private readonly IToDoService _service;
        public DelegateCommand AddToDoCommand { get; private set; }
        public DelegateCommand<ToDoDto> SelectedCommand { get; private set; }
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> DeleteCommand { get; private set; }
        private ObservableCollection<ToDoDto> _toDoTables;
        public ObservableCollection<ToDoDto> ToDoTables
        {
            get => _toDoTables;
            set
            {
                _toDoTables = value;
                this.RaisePropertyChanged(nameof(ToDoTables));
            }
        }
        private bool _rightDrawerOpen;
        /// <summary>
        /// 右侧边框栏是否展开
        /// </summary>
        public bool RightDrawerOpen
        {
            get => _rightDrawerOpen;
            set
            {
                _rightDrawerOpen = value;
                this.RaisePropertyChanged(nameof(RightDrawerOpen));
            }
        }
        private ToDoDto _currentToDo;
        /// <summary>
        /// 当前选中的TODO
        /// </summary>
        public ToDoDto CurrentToDo
        {
            get => _currentToDo;
            set
            {
                _currentToDo = value;
                this.RaisePropertyChanged(nameof(CurrentToDo));
            }
        }
        private string _search;
        /// <summary>
        /// 搜索条件
        /// </summary>
        public string Search
        {
            get => _search;
            set
            {
                _search = value;
                this.RaisePropertyChanged(nameof(Search));
            }
        }
        private int _selectedIndex;
        /// <summary>
        /// 待办事项状态索引(用于搜索)
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                this.RaisePropertyChanged(nameof(SelectedIndex));
            }
        }
        public ToDoViewModel(IToDoService service, IContainerProvider provider):base(provider)
        {
            ToDoTables = new ObservableCollection<ToDoDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            AddToDoCommand = new DelegateCommand(AddToDo);
            SelectedCommand = new DelegateCommand<ToDoDto>(Selected);
            DeleteCommand = new DelegateCommand<ToDoDto>(Delete);
            _service = service;
            //当服务在APP中注册了之后从容器中直接取出
            _Messagedialog = provider.Resolve<IDialogHostService>();
        }
        private async void Delete(ToDoDto obj)
        {
            //删除前的询问弹窗
            var dialogResult = await _Messagedialog.Question("温馨提示", $"是否确认删除当前待办事项:{obj.Title} ?");
            if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK)
            {
                return;
            }
             
            UpdateLoading(true);
            var deleteResult = await _service.DeleteAsync(obj.Id);
            if (deleteResult != null)
            {
                ToDoTables.Remove(obj);
            }
            UpdateLoading(false);
        }
        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增":AddToDo(); break;
                case "查询":GetDateAsync();break;
                case "保存":SaveToDo(); break;
            }
        }
        private async void SaveToDo()
        {
            if (string.IsNullOrWhiteSpace(CurrentToDo.Content) || string.IsNullOrWhiteSpace(CurrentToDo.Title))
            {
                MessageBox.Show("标题或内容不得为空");return;
            }
            UpdateLoading(true);
            try
            {
                if (CurrentToDo.Id > 0)
                {
                    //更新后端
                    var updateResult = await _service.UpdateAsync(CurrentToDo);
                    if (updateResult.Status)
                    {
                        //更新前端的集合
                        var toDo = ToDoTables.FirstOrDefault(t => t.Id.Equals(CurrentToDo.Id));
                        if (toDo != null)
                        {
                            toDo.Title = CurrentToDo.Title;
                            toDo.Content = CurrentToDo.Content;
                            toDo.UpdateTime = DateTime.Now;
                            toDo.Status = CurrentToDo.Status;
                        }
                    }
                    RightDrawerOpen = false;
                }
                else
                {
                    var addResult = await _service.AddAsync(CurrentToDo);
                    //这里的ID可以通过API的返回结果直接获得
                    if (addResult.Status)
                    {
                        ToDoTables.Add(addResult.Result);
                        RightDrawerOpen = false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            finally
            {
                UpdateLoading(false);
            }
        }
        private void AddToDo()
        {
            CurrentToDo = new ToDoDto();
            this.RightDrawerOpen = true;
        }
        private async void Selected(ToDoDto obj)
        {
            try
            {
                UpdateLoading(true);
                var selectedResult = await _service.GetFirstOfDefaultAsync(obj.Id);
                if (selectedResult.Status)
                {
                    CurrentToDo = selectedResult.Result;
                    this.RightDrawerOpen = true;
                }
            }
            catch (Exception e)
            {
                //后续增加日志记录
            }
            finally
            {
                UpdateLoading(false);
            }
        }
        /// <summary>
        /// 获取数据并增加了加载数据等待功能
        /// </summary>
        async private void GetDateAsync()
        {
            UpdateLoading(true);
            //对下拉框索引执行判断
            int? status = SelectedIndex == 0 ? null : SelectedIndex == 2 ? 1 : 0;
            var toDoResult = await _service.GetAllFilterAsync(new Shared.Parameters.ToDoParameter(){PageIndex = 0,PageSize = 100,Search = this.Search, Status = status });
            if (toDoResult.Status)
            {
                ToDoTables.Clear();
                foreach (var item in toDoResult.Result.Items)
                {
                    ToDoTables.Add(item);
                }
            }
            UpdateLoading(false);
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //获取导航参数
            if (navigationContext.Parameters.ContainsKey("Value"))
            {
                SelectedIndex = navigationContext.Parameters.GetValue<int>("Value");
            }
            else//如果没有参数这设置为默认值
            {
                SelectedIndex = 0;
            }
            base.OnNavigatedTo(navigationContext);
            //如果导航有参数则查询的时候就会按照当前SelectedIndex的值为条件去查询
            GetDateAsync();
        }
    }
}
