using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Components;
using course_oop.Presentation.Views.Admin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course_oop.Presentation.ViewModels.CouriersPart
{
    class CouriersMainVM : ViewModel, IMenu
    {
        private readonly ObservableCollection<MenuItemViewModel> _menuItems;

        private readonly Courier _courier;
        private readonly INavigation _navigator;

        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
        }

        public CouriersMainVM ( Courier courier, INavigation navigator, ViewModels.Registration.MainViewModel vm )
        {
            _courier = courier;
            _navigator = navigator;

            _navigator.Navigate(new Views.CourierView.Map(courier));

            _menuItems =
            [
                   new MenuItemViewModel("\xe826", "Маршруты", new Command(() => _navigator.Navigate(new Views.CourierView.Map(courier)))),
                 new("\xede1", "Выход", new Command(() => vm.GoBack()))
            ];
        }
    }
}
