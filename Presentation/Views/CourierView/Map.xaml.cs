using course_oop.Core.Entities;
using course_oop.Presentation.ViewModels.CouriersPart;
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

namespace course_oop.Presentation.Views.CourierView
{
    /// <summary>
    /// Логика взаимодействия для Map.xaml
    /// </summary>
    public partial class Map : Page
    {
        private readonly MapVM _mainViewModel;
        public Map(Courier param)
        {
            InitializeComponent();
            _mainViewModel = new(ref MapControl, param);
            this.DataContext = _mainViewModel;
            Loaded += MapPage_Loaded;
        }

        private async void MapPage_Loaded ( object sender, RoutedEventArgs e )
        {
            try
            {
                this.MapControl.Map = await _mainViewModel.MapServise.LoadMap();
                //if (_param != null)
                //{
                //    using AppContext appContext = new AppContext();
                //    _mainViewModel.Order = new OrderCardViewModel(_param, appContext.Products.Find(_param.ProductId)!);
                //}
                _mainViewModel.LoadingMsg = "Загрузка";
                await _mainViewModel.UpdteLayers();
            }
            catch (Exception ex)
            {
                _mainViewModel.LoadingMsg = ex.Message;
            }
        }

        private void ScrollViewer_PreviewMouseWheel ( object sender, MouseWheelEventArgs e )
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
}
