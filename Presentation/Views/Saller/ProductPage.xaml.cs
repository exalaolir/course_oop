using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using course_oop.Presentation.ViewModels.Components;

namespace course_oop.Presentation.Views.Saller
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private ProductViewModel _mainViewModel;

        public ProductPage(Product? product, Core.Entities.Saller saller, Shop shop)
        {
            InitializeComponent();
            _mainViewModel = new(product, saller, shop);
            this.DataContext = _mainViewModel;

            this.SizeChanged += (s, e) => { _mainViewModel.IsWideView = e.NewSize.Width > 1000; };

            this.SizeChanged += (s, e) =>
                _mainViewModel.IsWidePanel = e.NewSize.Width > 1200;
        }
    }
}