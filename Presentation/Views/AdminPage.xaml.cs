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
using System.Windows.Shapes;
using course_oop.Core.Entities;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.AdminPart;
using course_oop.Presentation.ViewModels.Components;
using course_oop.Presentation.ViewModels.UsersPart;

namespace course_oop.Presentation.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private readonly AdminViewModel _mainViewModel;

        public AdminPage(Core.Entities.Admin admin, ViewModels.Registration.MainViewModel vm )
        {
            InitializeComponent();
            _mainViewModel = new AdminViewModel(admin, new NavigationServise(AdminFrame), vm);
            this.DataContext = _mainViewModel;
        }

        private void ListViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is MenuItemViewModel menuItemViewModel)
            {
                menuItemViewModel.Command.Execute(null);
            }
        }
    }
}