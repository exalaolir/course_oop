using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Windows.Threading;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using Microsoft.Toolkit.Uwp.Notifications;

namespace course_oop.Presentation.ViewModels.Registration
{
    internal sealed class UserRegistrationViewModel : ValidatedViewModel, IDateTimer, IPassword, IRepeatPassword
    {
        private readonly MainViewModel _mainViewModel;

        public ICommand SetLogin => _mainViewModel.SetLogin;
        public ICommand SetBuisnesRegistration => _mainViewModel.SetBuisnesRegistration;


        private DateTime _currentDate;
        private readonly DispatcherTimer _timer;

        private string _firstName;
        private string _name;
        private string _email;
        private string _phone;
        private string _pasword;
        private string _repitedPassword;


        public DateTime CurrentDate
        {
            get => _currentDate;
            set => this.SetValue(ref _currentDate, value);
        }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"(^[a-zA-Z]+$)|(^[а-яА-Я]+$)", ErrorMessage = "Одно слово кириллицей или латиницей")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                this.SetValue(ref _firstName, value);
                Validate(FirstName);
                _validFields[0] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"(^[a-zA-Z]+$)|(^[а-яА-Я]+$)", ErrorMessage = "Одно слово кириллицей или латиницей")]
        public string Name
        {
            get => _name;
            set
            {
                this.SetValue(ref _name, value);
                Validate(Name);
                _validFields[1] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress(ErrorMessage = "Неверный email")]
        public string Email
        {
            get => _email;
            set
            {
                this.SetValue(ref _email, value);
                Validate(Email);
                _validFields[2] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^\+375\d{9}$", ErrorMessage = "Формат +375XXXXXXXXX")]
        public string Phone
        {
            get => _phone;
            set
            {
                this.SetValue(ref _phone, value);
                Validate(Phone);
                _validFields[3] = true;
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
            get => _pasword;
            set
            {
                this.SetValue(ref _pasword, value);
                Validate(Password, type: ValidationType.Password);
                if (Password.Length >= 8)
                    Validate(RepeatPassword, type: ValidationType.RepeatPassword, nameof(RepeatPassword));
                _validFields[4] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }


        [Required(ErrorMessage = "Обязательное поле")]
        [Compare(nameof(Password), ErrorMessage = "Пароли должны совпадать")]
        [RegularExpression(@"^(?=.*[A-Z].*)(?=.*[!@#$&*])(?=.*[0-9].*)(?=.*[a-z].*).{8,}$", ErrorMessage = """
            Минимум 8 символов.
            Латиница. 
            Минимум 1 прописная буква, цифра и символ из: !@#$&*
            """)]
        public string RepeatPassword
        {
            get => _repitedPassword;
            set
            {
                this.SetValue(ref _repitedPassword, value);
                Validate(RepeatPassword, type: ValidationType.RepeatPassword);
                _validFields[5] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public UserRegistrationViewModel(MainViewModel mainViewModel)
        {
            _currentDate = DateTime.Today;
            _mainViewModel = mainViewModel;
            _timer = new DispatcherTimer
            {
                Interval = IDateTimer.CalculateTimeUntilMidnight()
            };
            _timer.Tick += OnDayChanged;
            _timer.Start();

            _firstName = string.Empty;
            _name = string.Empty;
            _email = string.Empty;
            _phone = string.Empty;
            _pasword = string.Empty;
            _repitedPassword = string.Empty;

            Register = new Command(OnRegister);
            _validFields = new bool[6];
        }

        public void OnDayChanged(object? sender, EventArgs e)
        {
            CurrentDate = DateTime.Today;
            _timer.Interval = IDateTimer.CalculateTimeUntilMidnight();
        }

        public ICommand Register { get; }

        private void OnRegister()
        {
            User user = new()
            {
                Name = _name,
                FirstName = _firstName,
                Email = _email,
                Phone = _phone,
                Password = PasswordHasher.HashPassword(_pasword),
                Banned = false,
            };

            var validResults = ValidationServis.ValidateUser(user);

            foreach (var result in validResults)
            {
                SetValidationResults(result.result, result.property, result.message ?? []);
            }

            if (!HasErrors)
            {
                _mainViewModel.Authentificator.AddUser(user);
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("Успех")
                    .AddText("Регистрация завершена")
                    .Show();
                _mainViewModel.SetLogin.Execute(this);
            }
        }
    }
}