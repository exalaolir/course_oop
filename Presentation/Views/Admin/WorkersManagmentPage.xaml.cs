using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.AdminPart;

namespace course_oop.Presentation.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для WorkersManagmentPage.xaml
    /// </summary>
    public partial class WorkersManagmentPage : Page
    {
        private readonly WorkersViewModelcs _mainViewModel;

        public WorkersManagmentPage()
        {
            InitializeComponent();
            _mainViewModel = new WorkersViewModelcs();
            this.DataContext = _mainViewModel;
        }

        private void SetPassword(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.Registration is IPassword passProperty && sender is PasswordBox box)
            {
                passProperty.Password = box.Password;
            }
        }

        private void SetRepeatPassword(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.Registration is IRepeatPassword passProperty && sender is PasswordBox box)
            {
                passProperty.RepeatPassword = box.Password;
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                if (tabControl.SelectedIndex == 1)
                    this.DataContext = _mainViewModel.Registration;
                else
                    this.DataContext = _mainViewModel;
            }
        }
    }
}