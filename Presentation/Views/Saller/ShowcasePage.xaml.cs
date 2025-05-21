using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using course_oop.Core.Entities;
using course_oop.Presentation.ViewModels.SallersPart;

namespace course_oop.Presentation.Views.Saller
{
    /// <summary>
    /// Логика взаимодействия для ShowcasePage1xaml.xaml
    /// </summary>
    public partial class ShowcasePage : Page
    {
        private readonly ShowcaseViewModel _mainViewModel;

        public ShowcasePage(Core.Entities.Saller saller)
        {
            InitializeComponent();
            _mainViewModel = new(saller);
            this.DataContext = _mainViewModel;

            this.SizeChanged += (s, e) => { _mainViewModel.IsWideView = e.NewSize.Width > 1000; };

            this.SizeChanged += (s, e) =>
                _mainViewModel.IsWidePanel = e.NewSize.Width > 1200;
        }
    }
}