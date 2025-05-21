using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using Microsoft.Data.SqlClient;

namespace course_oop.Presentation.ViewModels.Registration
{
    public sealed class MainViewModel : ViewModel, INavigated
    {
        private object _curentViewModel;
        private readonly ValidationServis? _validator;
        private readonly AppContext? _context;
        private readonly AuthentificationServies? _authentificationServies;
        private readonly ViewModels.MainViewModel _mainViewModel;

        public void GoBack() => _mainViewModel.GoBack();

        public object CurentViewModel
        {
            get => _curentViewModel;
            set => this.SetValue(ref _curentViewModel, value);
        }

        internal ValidationServis Validator
        {
            get => _validator ??
                   throw new ArgumentNullException(this.ToString(), "Вызван валидатор, при ошибке подключения к бд");
        }

        internal AuthentificationServies Authentificator
        {
            get => _authentificationServies ??
                   throw new ArgumentNullException(this.ToString(),
                       "Вызван аутентификатор, при ошибке подключения к бд");
        }

        public MainViewModel( ViewModels.MainViewModel mainViewModel )
        {
            _curentViewModel = new LoginViewModel(this);
            _mainViewModel = mainViewModel;

            SetLogin = new Command(() => CurentViewModel = new LoginViewModel(this));

            SetUserRegistration = new Command(() => CurentViewModel = new UserRegistrationViewModel(this));

            SetBuisnesRegistration = new Command(() => CurentViewModel = new BisnesRegistrationViewModel(this));

            try
            {
                _context = new AppContext();
                _validator = new ValidationServis();
                _authentificationServies = new();
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public ICommand SetLogin { get; }
        public ICommand SetUserRegistration { get; }
        public ICommand SetBuisnesRegistration { get; }
        public INavigation? Navigator { get; set; }
    }
}