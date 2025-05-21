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
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly HomePageViewModel _vm;
        public HomePage(Core.Entities.User user)
        {
            InitializeComponent();
            _vm = new(user);
            this.DataContext = _vm;
        }
    }
}