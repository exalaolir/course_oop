using System.Windows;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels;

namespace course_oop.Presentation.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            NavigationServise navigator = new(MainFrame);
            _mainViewModel = new MainViewModel(navigator);
        }
    }
}