using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using course_oop.Core.Interfaces;

namespace course_oop.Core.Services
{
    internal sealed class NavigationServise : INavigation
    {
        private readonly Frame _frame;

        public NavigationServise(Frame frame)
        {
            _frame = frame;
            _frame.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page && page.DataContext is INavigated veiwModel)
            {
                veiwModel.Navigator = this;
            }
        }

        public void Navigate<T>(T page) where T : Page
        {
            _frame.Navigate(page);
            _frame.NavigationService.RemoveBackEntry();
        }

        public void Navigate(string pageName)
        {
            var uri = new System.Uri($"Presentation/Views/{pageName}.xaml", UriKind.Relative);
            _frame.Navigate(uri);
            _frame.NavigationService.RemoveBackEntry();
        }
    }
}