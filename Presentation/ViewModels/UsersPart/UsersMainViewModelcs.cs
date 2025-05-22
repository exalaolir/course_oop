using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Components;
using course_oop.Presentation.ViewModels.Registration;
using course_oop.Presentation.Views.User;

namespace course_oop.Presentation.ViewModels.UsersPart
{
    public sealed class UsersMainViewModelcs : ViewModel, IMenu
    {
        private readonly ObservableCollection<MenuItemViewModel> _menuItems;

        public User _user;
        private readonly INavigation _navigator;

        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
        }

        public UsersMainViewModelcs ( User user, INavigation navigator, ViewModels.Registration.MainViewModel vm )
        {
            _user = user;
            _navigator = navigator;
            _navigator.Navigate(new HomePage(_user));

            _menuItems =
            [
                new("\xEA8A", "Главная", new Command(() => _navigator.Navigate(new HomePage(_user)))),
                  new("\xe8a9", "Каталог", new Command(() => _navigator.Navigate(new CatalogPage(_user)))),
                new("\xe7ee", "Профиль", new Command(() => _navigator.Navigate(new InfoPage(ref _user, this)))),
                new("\xEB4E", "Корзина", new Command(() => _navigator.Navigate(new BasketPage(_user)))),
                 new("\xede1", "Выход", new Command(() => vm.GoBack()))

            ];
        }
    }
}