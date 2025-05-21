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
using course_oop.Presentation.ViewModels.AdminPart;
using Mapsui;

namespace course_oop.Presentation.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для MapPage.xaml
    /// </summary>
    public partial class MapPage : Page
    {
        private readonly MapViewModel _mainViewModel;
        private readonly Order? _param;

        public MapPage(Order? param = null)
        {
            InitializeComponent();
            _param = param;
            _mainViewModel = new(ref MapControl, null);
            this.DataContext = _mainViewModel;
            Loaded += MapPage_Loaded;
        }

        private async void MapPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.MapControl.Map = await _mainViewModel.MapServise.LoadMap();
                if(_param != null)
                {
                    using AppContext appContext = new AppContext();
                    _mainViewModel.Order = new OrderCardViewModel(_param, appContext.Products.Find(_param.ProductId)!);
                }
                _mainViewModel.DataLoaded = true;
                _mainViewModel.LoadingMsg = "Загрузка";
            }
            catch (Exception ex)
            {
                _mainViewModel.LoadingMsg = ex.Message;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
}