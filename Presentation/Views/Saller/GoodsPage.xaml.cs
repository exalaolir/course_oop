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
using course_oop.Presentation.ViewModels.SallersPart;

namespace course_oop.Presentation.Views.Saller
{
    /// <summary>
    /// Логика взаимодействия для GoodsPage.xaml
    /// </summary>
    public partial class GoodsPage : Page
    {
        private readonly GoodsViewModel _mainViewModel;

        public GoodsPage(Core.Entities.Saller saller)
        {
            InitializeComponent();
            _mainViewModel = new(saller);
            this.DataContext = _mainViewModel;
        }
    }
}