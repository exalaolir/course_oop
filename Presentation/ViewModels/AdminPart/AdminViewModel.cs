using System.Collections.ObjectModel;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Components;
using course_oop.Presentation.Views.Admin;
using Microsoft.Toolkit.Uwp.Notifications;

namespace course_oop.Presentation.ViewModels.AdminPart
{
    internal class AdminViewModel : ViewModel, IMenu
    {
        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
        }

        private readonly ObservableCollection<MenuItemViewModel> _menuItems;

        private Admin _admin;
        private readonly INavigation _navigator;

        public AdminViewModel(Admin admin, INavigation navigator, ViewModels.Registration.MainViewModel vm )
        {
            _admin = admin;
            _navigator = navigator;

            _menuItems =
            [
                new MenuItemViewModel("\xeb7e", "Категории", new Command(() => _navigator.Navigate(new Categoris()))),
                new MenuItemViewModel("\xe826", "Маршруты", new Command(() => _navigator.Navigate(new MapPage()))),
                new MenuItemViewModel("\xe7ee", "Персонал",
                    new Command(() => _navigator.Navigate(new WorkersManagmentPage()))),
                new MenuItemViewModel("\xf2a5", "Заявки", new Command(() => _navigator.Navigate(new InvitesPage()))),
                 new("\xede1", "Выход", new Command(() => vm.GoBack()))
            ];

            using Repo repository = new();
            if (repository.GetUnVishedSealers() != null)
            {
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("Внимание")
                    .AddText("Доступны новые заявки на регистрацию")
                    .Show();
            }
        }
    }
}