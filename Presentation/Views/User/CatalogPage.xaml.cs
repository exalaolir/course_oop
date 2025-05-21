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
using course_oop.Core.Entities;
using course_oop.Presentation.ViewModels.UsersPart;

namespace course_oop.Presentation.Views.User
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private CatalogViewModel _mainViewModel;

        public CatalogPage(Core.Entities.User user)
        {
            InitializeComponent();
            _mainViewModel = new(user);
            this.DataContext = _mainViewModel;
        }
    }
}