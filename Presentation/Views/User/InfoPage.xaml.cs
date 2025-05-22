using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.UsersPart;
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

namespace course_oop.Presentation.Views.User
{
    /// <summary>
    /// Логика взаимодействия для InfoPage.xaml
    /// </summary>
    public partial class InfoPage : Page
    {
        private InfoViewModel _mainViewModel;
        public InfoPage(ref Core.Entities.User user, UsersMainViewModelcs vm)
        {
            InitializeComponent();
            _mainViewModel = new(ref user, vm);
            this.DataContext = _mainViewModel;
        }

        private void SetPassword ( object sender, RoutedEventArgs e )
        {
            if (_mainViewModel is IPassword passProperty && sender is PasswordBox box)
            {
                passProperty.Password = box.Password;
            }
        }

        private void SetRepeatPassword ( object sender, RoutedEventArgs e )
        {
            if (_mainViewModel is IRepeatPassword passProperty && sender is PasswordBox box)
            {
                passProperty.RepeatPassword = box.Password;
            }
        }
    }
}