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
    /// Логика взаимодействия для BasketPage.xaml
    /// </summary>
    public partial class BasketPage : Page
    {
        private readonly BasketViewModel _mainViewModel;

        public BasketPage(Core.Entities.User user, Order? order = null)
        {
            InitializeComponent();
            _mainViewModel = new(user, order);
            this.DataContext = _mainViewModel;
        }
    }
}