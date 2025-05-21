using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Components;
using course_oop.Presentation.Views.Saller;

namespace course_oop.Presentation.ViewModels.SallersPart
{
    class SallerViewModel : ViewModel, IMenu
    {
        readonly Registration.MainViewModel _vm;
        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
        }

        private readonly ObservableCollection<MenuItemViewModel> _menuItems;

        private Saller _saller;
        private readonly INavigation _navigator;

        public SallerViewModel ( Saller saller, INavigation navigator, ViewModels.Registration.MainViewModel vm )
        {
            _saller = saller;
            _navigator = navigator;
            _vm = vm;

            _menuItems =
            [
                new MenuItemViewModel("\xeb95", "Магазин",
                    new Command(() => _navigator.Navigate(new ShowcasePage(saller)))),
                new MenuItemViewModel("\xe9a4", "Товары",
                    new Command(() => _navigator.Navigate(new GoodsPage(saller)))),
                new MenuItemViewModel("\xefff", "Заказы", new Command(() => _navigator.Navigate(new Orders(saller)))),
                 new("\xede1", "Выход", new Command(() => _vm.GoBack()))
            ];
        }
    }
}