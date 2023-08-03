using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyToDo.Common;
using MyToDo.Common.Models;
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
    public class MemoViewModel: NavigationViewModel
    {
        private readonly IMemoService _service;
        private readonly IDialogHostService _Messagedialog;
        public DelegateCommand AddMemoCommand { get; private set; }
        public DelegateCommand<MemoDto> SelectedCommand { get; private set; }
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<MemoDto> DeleteCommand { get; private set; }
        private ObservableCollection<MemoDto> _memoTables;
        public ObservableCollection<MemoDto> MemoTables
        {
            get => _memoTables;
            set
            {
                _memoTables = value;
                this.RaisePropertyChanged(nameof(MemoTables));
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
        private MemoDto _currentMemo;
        /// <summary>
        /// 当前选中的memo
        /// </summary>
        public MemoDto CurrentMemo
        {
            get => _currentMemo;
            set
            {
                _currentMemo = value;
                this.RaisePropertyChanged(nameof(CurrentMemo));
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
        public MemoViewModel(IMemoService service, IContainerProvider provider) : base(provider)
        {
            MemoTables = new ObservableCollection<MemoDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            AddMemoCommand = new DelegateCommand(AddToDo);
            SelectedCommand = new DelegateCommand<MemoDto>(Selected);
            DeleteCommand = new DelegateCommand<MemoDto>(Delete);
            _service = service;
            _Messagedialog = provider.Resolve<IDialogHostService>();
        }
        private async void Delete(MemoDto obj)
        {
            var dialogReuslt = await _Messagedialog.Question("温馨提示", $"是否确认删除当前备忘录:{obj.Title} ?");
            if (dialogReuslt.Result != ButtonResult.OK)
            {
                return;
            }
            UpdateLoading(true);
            var deleteResult = await _service.DeleteAsync(obj.Id);
            if (deleteResult != null)
            {
                MemoTables.Remove(obj);
            }
            UpdateLoading(false);
        }
        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增": AddToDo(); break;
                case "查询": GetDateAsync(); break;
                case "保存": SaveMemo(); break;
            }
        }
        private async void SaveMemo()
        {
            if (string.IsNullOrWhiteSpace(CurrentMemo.Content) || string.IsNullOrWhiteSpace(CurrentMemo.Title))
            {
                MessageBox.Show("标题或内容不得为空"); return;
            }
            UpdateLoading(true);
            try
            {
                if (CurrentMemo.Id > 0)
                {
                    //更新后端
                    var updateResult = await _service.UpdateAsync(CurrentMemo);
                    if (updateResult.Status)
                    {
                        //更新前端的集合
                        var toDo = MemoTables.FirstOrDefault(t => t.Id.Equals(CurrentMemo.Id));
                        if (toDo != null)
                        {
                            toDo.Title = CurrentMemo.Title;
                            toDo.Content = CurrentMemo.Content;
                            toDo.UpdateTime = DateTime.Now;
                        }
                    }
                    RightDrawerOpen = false;
                }
                else
                {
                    var addResult = await _service.AddAsync(CurrentMemo);
                    //这里的ID可以通过API的返回结果直接获得
                    if (addResult.Status)
                    {
                        MemoTables.Add(addResult.Result);
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
            CurrentMemo = new MemoDto();
            this.RightDrawerOpen = true;
        }
        private async void Selected(MemoDto obj)
        {
            try
            {
                UpdateLoading(true);
                var selectedResult = await _service.GetFirstOfDefaultAsync(obj.Id);
                if (selectedResult.Status)
                {
                    CurrentMemo = selectedResult.Result;
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
            var toDoResult = await _service.GetAllAsync(new Shared.Parameters.QueryParameter() { PageIndex = 0, PageSize = 100, Search = this.Search});
            if (toDoResult.Status)
            {
                MemoTables.Clear();
                foreach (var item in toDoResult.Result.Items)
                {
                    MemoTables.Add(item);
                }
            }
            UpdateLoading(false);
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            GetDateAsync();
        }
    }
}
