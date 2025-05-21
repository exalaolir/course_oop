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
using System.Windows.Threading;
using course_oop.Presentation.ViewModels.UsersPart;

namespace course_oop.Presentation.Views.User
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private readonly ProductViewModel _mainViewModel;

        public ProductPage(Core.Entities.User user, CardViewModel card)
        {
            InitializeComponent();
            _mainViewModel = new(card, user);
            this.DataContext = _mainViewModel;
            this.Loaded += (s, e) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _mainViewModel.ImageWidth = this.ActualHeight > 900 && this.ActualWidth > 1200 ? 700 : 500;
                    this.SizeChanged += (s, e) =>
                    {
                        _mainViewModel.ImageWidth = e.NewSize.Height > 900 && e.NewSize.Width > 1200 ? 700 : 500;
                    };
                }), DispatcherPriority.ContextIdle);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            parentMenuItem.IsSubmenuOpen = false;
        }
    }
}