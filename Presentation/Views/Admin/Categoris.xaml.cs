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
using course_oop.Presentation.ViewModels.AdminPart;

namespace course_oop.Presentation.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для Categoris.xaml
    /// </summary>
    public partial class Categoris : Page
    {
        private readonly CategirisViewModel _mainViewModel;

        public Categoris()
        {
            InitializeComponent();
            _mainViewModel = new();
            this.DataContext = _mainViewModel;
        }
    }
}