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
using course_oop.Presentation.ViewModels.Registration;

namespace course_oop.Presentation.Views
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private readonly MainViewModel _mainViewModel;

        public RegistrationPage(Presentation.ViewModels.MainViewModel vm )
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel(vm);
            this.DataContext = _mainViewModel;
        }

        private void SetPassword(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.CurentViewModel is IPassword passProperty && sender is PasswordBox box)
            {
                passProperty.Password = box.Password;
            }
        }

        private void SetRepeatPassword(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.CurentViewModel is IRepeatPassword passProperty && sender is PasswordBox box)
            {
                passProperty.RepeatPassword = box.Password;
            }
        }
    }
}