using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MyToDo.ViewModels
{
    public class SettingViewModel:BindableBase
    {
        public DelegateCommand<MenuBar> navigateCommand { get; private set; }
        public readonly IRegionManager regionManager;
        private ObservableCollection<MenuBar> _menuBars;
        public ObservableCollection<MenuBar> MenuBars
        {
            get => _menuBars;
            set
            {
                _menuBars = value;
                this.RaisePropertyChanged(nameof(MenuBars));
            }
        }
        public SettingViewModel(IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            this.regionManager = regionManager;
            navigateCommand = new DelegateCommand<MenuBar>(Navigate);
            CreateMenuBar();
        }

        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.NameSpace)) return;
            regionManager.Regions[PrismManager.SettingsViewRegionName].RequestNavigate(obj.NameSpace);
        }


        private void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "PaletteOutline", Title = "个性化", NameSpace = "SkinView" });
            MenuBars.Add(new MenuBar() { Icon = "Tune", Title = "系统设置", NameSpace = "aaa" });
            MenuBars.Add(new MenuBar() { Icon = "InformationVariantCircleOutline", Title = "关于更多", NameSpace = "AboutView" });
        }
        
        
    }
}
