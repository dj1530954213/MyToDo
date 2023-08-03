using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace MyToDo.ViewModels
{
    public class IndexViewModel:NavigationViewModel
    {
        #region 属性及字段
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> EditToDoCommand { get; private set; }
        public DelegateCommand<MemoDto> EditMemoCommand { get; private set; }
        public DelegateCommand<ToDoDto> ToDoCompltedCommand { get; private set; }
        public DelegateCommand<TaskBar> NavigateCommand { get; private set; }
        

        private ObservableCollection<TaskBar> _taskBars;
        public ObservableCollection<TaskBar> TaskBars
        {
            get => _taskBars;
            set
            {
                _taskBars = value;
                this.RaisePropertyChanged(nameof(TaskBars));
            }

        }
        
        private SummaryDto _summary;
        public SummaryDto Summary
        {
            get => _summary;
            set
            {
                _summary = value;
                this.RaisePropertyChanged(nameof(Summary));
            }
        }

        private string _currentDate;
        public string CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                this.RaisePropertyChanged(nameof(CurrentDate));
            }
        }

        private readonly IDialogHostService _dialogService;
        private readonly IToDoService _toDoService;
        private readonly IMemoService _memoService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _aggregator;
        #endregion
        public IndexViewModel(IDialogHostService dialogService, IContainerProvider containerProvider):base(containerProvider)
        {
            _dialogService = dialogService;
            ExecuteCommand = new DelegateCommand<string>(Execute);
            EditToDoCommand = new DelegateCommand<ToDoDto>(AddToDo);
            EditMemoCommand = new DelegateCommand<MemoDto>(AddMemo);
            ToDoCompltedCommand = new DelegateCommand<ToDoDto>(ToDoComplted);
            NavigateCommand = new DelegateCommand<TaskBar>(NavigateHandle);
            TaskBars = new ObservableCollection<TaskBar>();
            //ToDoDtos = new ObservableCollection<ToDoDto>();
            //MemoDtos = new ObservableCollection<MemoDto>();
            _toDoService = containerProvider.Resolve<IToDoService>();
            _memoService = containerProvider.Resolve<IMemoService>();
            _regionManager = containerProvider.Resolve<IRegionManager>();
            _aggregator = containerProvider.Resolve<IEventAggregator>();
            this.CurrentDate = $"您好{AppSession.UserName}！今天是{DateTime.Now.Year.ToString()}年{DateTime.Now.Month.ToString()}月{DateTime.Now.Day.ToString()}日";
            CreatTaskBar();
        }

        private void NavigateHandle(TaskBar obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Target))
            {
                return;
            }
            NavigationParameters parameters = new NavigationParameters();
            //向导航到的页面传递参数，完成状态选择框的索引为2所以传递2过去
            if (obj.Title == "已完成")
            {
                parameters.Add("Value",2);
            }
            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.Target,parameters);
        }

        private async void ToDoComplted(ToDoDto obj)
        {
            UpdateLoading(true);
            var updateResult = await _toDoService.UpdateAsync(obj);
            if (updateResult.Status)
            {
                var result = Summary.ToDoList.FirstOrDefault(t => t.Id.Equals(obj.Id));
                if (result != null)
                {
                    Summary.ToDoList.Remove(result);
                    Summary.CompletedCount += 1;
                    Summary.CompletedRatio = (Summary.CompletedCount / (double)Summary.Sum).ToString("0%");
                    this.TaskBarRefresh();
                }
            }
            _aggregator.SendMessage($"待办事项:{obj.Title}  已完成");
            UpdateLoading(false);
        }
        private void CreatTaskBar()
        {
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Color = "#FF0CA0FF", Target = "ToDoView", Title = "汇总" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Color = "#FF1ECA3A", Target = "ToDoView", Title = "已完成" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Color = "#FF02C6DC", Target = "", Title = "完成比例" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Color = "#FFFFA000", Target = "MemoView", Title = "备忘录" });
        }
        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增待办事项":
                    AddToDo(null);break;
                case "新增备忘录":
                    AddMemo(null); break;
            }
        }
        /// <summary>
        /// indexView中的todo新增和修改方法
        /// </summary>
        /// <param name="todo"></param>
        public async void AddToDo(ToDoDto todo)
        {
            //如果todo不为空说明是编辑模式则将前端传回的todo通过DialogParameters传递值弹窗中
            DialogParameters parm = new DialogParameters();
            if (todo != null)
            {
                parm.Add("Value",todo);
            }
            var dialogResult = await _dialogService.ShowDialog("AddToDoView", parm);
            if (dialogResult.Result != ButtonResult.OK)
            {
                return;
            }
            var toDoDate = dialogResult.Parameters.GetValue<ToDoDto>("Value");
            //根据是否有id判断是否为新增或更新
            if (toDoDate.Id>0)
            {
                UpdateLoading(true);
                 var updateResult = await _toDoService.UpdateAsync(toDoDate);
                 if (updateResult.Status)
                 {
                     var result = Summary.ToDoList.FirstOrDefault(t => t.Id.Equals(toDoDate.Id));
                     if (result != null)
                     {
                         result.Title = toDoDate.Title;
                         result.Content = toDoDate.Content;
                     }
                 }
                 UpdateLoading(false);
            }
            else
            {
                UpdateLoading(true);
                var addResult = await _toDoService.AddAsync(toDoDate);
                if (addResult.Status)
                {
                    //在首页添加新的待办事项之后需要更新汇总的数据
                    Summary.Sum += 1;
                    Summary.CompletedRatio = (Summary.CompletedCount / (double)Summary.Sum).ToString("0%");
                    //如果添加toDoDate那就不是从后端返回的带ID的数据所以需要添加Result
                    Summary.ToDoList.Add(addResult.Result);
                    this.TaskBarRefresh();
                }
                UpdateLoading(false);
            }
            
        }
        /// <summary>
        /// indexView中的memo新增和修改方法
        /// </summary>
        /// <param name="memo"></param>
        public async void AddMemo(MemoDto memo)
        {
            //如果todo不为空说明是编辑模式则将前端传回的memo通过DialogParameters传递值弹窗中
            DialogParameters parm = new DialogParameters();
            if (memo != null)
            {
                parm.Add("Value", memo);
            }
            var dialogResult = await _dialogService.ShowDialog("AddMemoView", parm);
            if (dialogResult.Result != ButtonResult.OK)
            {
                return;
            }
            var memoDate = dialogResult.Parameters.GetValue<MemoDto>("Value");
            //根据是否有id判断是否为新增或更新
            if (memoDate.Id > 0)
            {
                UpdateLoading(true);
                var updateResult = await _memoService.UpdateAsync(memoDate);
                if (updateResult.Status)
                {
                    var result = Summary.MemoList.FirstOrDefault(t => t.Id.Equals(memoDate.Id));
                    if (result != null)
                    {
                        result.Title = memoDate.Title;
                        result.Content = memoDate.Content;
                    }
                }
                UpdateLoading(false);
            }
            else
            {
                UpdateLoading(true);
                var addResult = await _memoService.AddAsync(memoDate);
                if (addResult.Status)
                {
                    Summary.MemoCount += 1;
                    this.TaskBarRefresh();
                    //如果添加toDoDate那就不是从后端返回的带ID的数据
                    Summary.MemoList.Add(addResult.Result);
                }
                UpdateLoading(false);
            }
            
        }
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var summaryResult = await _toDoService.SummaryAsync();
            if (summaryResult.Status)
            {
                Summary = summaryResult.Result;
            }
            base.OnNavigatedTo(navigationContext);
            GetDateAsync();
            TaskBarRefresh();
        }
        private async void GetDateAsync()
        {
            UpdateLoading(true);
            //对下拉框索引执行判断
            var toDoResult = await _toDoService.GetAllFilterAsync(new Shared.Parameters.ToDoParameter() { PageIndex = 0, PageSize = 100});
            if (toDoResult.Status)
            {
                Summary.ToDoList.Clear();
                foreach (var item in toDoResult.Result.Items)
                {
                    if (item.Status == 0)
                    {
                        Summary.ToDoList.Add(item);
                    }
                }
            }
            var memoResult = await _memoService.GetAllAsync(new Shared.Parameters.ToDoParameter() { PageIndex = 0, PageSize = 100 });
            if (memoResult.Status)
            {
                Summary.MemoList.Clear();
                foreach (var item in memoResult.Result.Items)
                {
                    Summary.MemoList.Add(item);
                }
            }
            UpdateLoading(false);
        }
        void TaskBarRefresh()
        {
            TaskBars[0].Content = Summary.Sum.ToString();
            TaskBars[1].Content = Summary.CompletedCount.ToString();
            TaskBars[2].Content = Summary.CompletedRatio;
            TaskBars[3].Content = Summary.MemoCount.ToString();
        }
    }
}
