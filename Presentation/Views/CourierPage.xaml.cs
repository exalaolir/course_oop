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
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.Components;
using course_oop.Presentation.ViewModels.CouriersPart;

namespace course_oop.Presentation.Views
{
    /// <summary>
    /// Логика взаимодействия для CourierPage.xaml
    /// </summary>
    public partial class CourierPage : Page
    {
        private readonly CouriersMainVM _couriersMainVM;
        public CourierPage(Courier courier, ViewModels.Registration.MainViewModel vm )
        {
            InitializeComponent();
            _couriersMainVM = new(courier, new NavigationServise(CouriersFrame), vm);
            this.DataContext = _couriersMainVM;
        }

        private void ListViewItem_MouseLeftButtonUp ( object sender, MouseButtonEventArgs e )
        {
            if (sender is ListViewItem item && item.DataContext is MenuItemViewModel menuItemViewModel)
            {
                menuItemViewModel.Command.Execute(null);
            }
        }
    }
}