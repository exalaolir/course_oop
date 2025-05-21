using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views;
using course_oop.Shared.Const;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Toolkit.Uwp.Notifications;


namespace course_oop.Presentation.ViewModels.Registration
{
    internal sealed class LoginViewModel : ValidatedViewModel, IPassword
    {
        private readonly MainViewModel _mainViewModel;

        private string _email;
        private string _password;

        public ICommand SetUserRegistration => _mainViewModel.SetUserRegistration;
        public ICommand Login { get; }

        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress(ErrorMessage = "Неверный email")]
        public string Email
        {
            get => _email;
            set
            {
                this.SetValue(ref _email, value);
                Validate(Email);
                _validFields[0] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^(?=.*[A-Z].*)(?=.*[!@#$&*])(?=.*[0-9].*)(?=.*[a-z].*).{8,}$", ErrorMessage = """
            Минимум 8 символов.
            Латиница. 
            Минимум 1 прописная буква, цифра и символ из: !@#$&*
            """)]
        public string Password
        {
            get => _password;

            set
            {
                this.SetValue(ref _password, value);
                Validate(Password, type: ValidationType.Password);
                _validFields[1] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public LoginViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _email = string.Empty;
            _password = string.Empty;

            _validFields = new bool[2];

            Login = new Command(OnLogin);
        }

        private void OnLogin()
        {
            var authSuccessed = _mainViewModel.Authentificator.Authentificate(Email, Password, out var results);

            if (authSuccessed == Consts.AuthResults.Success)
            {
                switch (results!.Value.role)
                {
                    case Consts.Roles.User:
                        var user = (User)results.Value.user;
                        _mainViewModel.Navigator?.Navigate(
                            new UserPage(user, _mainViewModel)
                        );
                        break;

                    case Consts.Roles.Saler:
                        var saller = (Saller)results.Value.user;
                        _mainViewModel.Navigator?.Navigate(
                            new SallerPage(saller, _mainViewModel)
                        );
                        break;

                    case Consts.Roles.Admin:
                        var admin = (Admin)results.Value.user;
                        _mainViewModel.Navigator?.Navigate(
                            new AdminPage(admin, _mainViewModel)
                        );
                        break;

                    case Consts.Roles.Courier:
                        var courier = (Courier)results.Value.user;
                        _mainViewModel.Navigator?.Navigate(
                            page: new CourierPage(courier, _mainViewModel)
                        );
                        break;
                }
            }
            else if (authSuccessed == Consts.AuthResults.EmailError)
            {
                this.SetValidationResults(false, nameof(Email), ["Невеный email"]);
                _validFields[0] = false;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
            else
            {
                this.SetValidationResults(false, nameof(Password), ["Невеный пароль"]);
                HasPasswordErrors = true;
                PasswordError = _errors[nameof(Password)][0];
                _validFields[1] = false;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }
    }
}