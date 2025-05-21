using course_oop.Core.Interfaces;
using course_oop.Presentation.Views;

namespace course_oop.Presentation.ViewModels
{
    public class MainViewModel
    {
        private readonly INavigation _navigator;

        public MainViewModel(INavigation navigator)
        {
            _navigator = navigator;
            _navigator.Navigate(new RegistrationPage(this));
        }

        public void GoBack() => _navigator.Navigate(new RegistrationPage(this));
    }
}