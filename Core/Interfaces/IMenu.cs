using System.Collections.ObjectModel;
using course_oop.Presentation.ViewModels.Components;

namespace course_oop.Core.Interfaces
{
    internal interface IMenu
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    }
}