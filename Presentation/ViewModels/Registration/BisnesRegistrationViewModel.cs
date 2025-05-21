using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using Microsoft.Toolkit.Uwp.Notifications;


namespace course_oop.Presentation.ViewModels.Registration
{
    class BisnesRegistrationViewModel : ValidatedViewModel, IDateTimer, IPassword, IRepeatPassword
    {
        private readonly MainViewModel _mainViewModel;
        public ObservableCollection<Category> Categories { get; }


        public ICommand SetLogin => _mainViewModel.SetLogin;

        public ICommand SetUserRegistration => _mainViewModel.SetUserRegistration;

        private Category? _category;

        public BisnesRegistrationViewModel(MainViewModel mainViewModel)
        {
            using Repo repo = new();

            Categories = new(repo.GetCategories().Cast<Category>().Where(c => c.ParentId == null));

            _mainViewModel = mainViewModel;

            _firstName = string.Empty;
            _name = string.Empty;
            _email = string.Empty;
            _phone = string.Empty;
            _pasword = string.Empty;
            _repitedPassword = string.Empty;
            _number = string.Empty;


            _validFields = new bool[8];
            Register = new Command(OnRegister);
        }


        private string _firstName;
        private string _name;
        private string _email;
        private string _phone;
        private string _pasword;
        private string _repitedPassword;
        private string _number;

        public Category? Category
        {
            get => _category;
            set
            {
                this.SetValue(ref _category, value);
                _validFields[7] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
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
        [Compare(nameof(Password), ErrorMessage = "Пароли должны совпадать")]
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

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Формат XXXXXXXXX")]
        public string SallerId
        {
            get => _number;
            set
            {
                this.SetValue(ref _number, value);
                Validate(SallerId);
                _validFields[6] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public ICommand Register { get; }

        private void OnRegister()
        {
            Saller saller = new()
            {
                Name = _name,
                FirstName = _firstName,
                Email = _email,
                Phone = _phone,
                Password = PasswordHasher.HashPassword(_pasword),
                Banned = false,
                SalersId = _number,
                CategoryId = _category?.Id,
            };

            var validResults = _mainViewModel.Validator.ValidateSaler(saller);

            foreach (var result in validResults)
            {
                SetValidationResults(result.result, result.property, result.message ?? []);
            }

            if (!HasErrors)
            {
                _mainViewModel.Authentificator.AddSaller(saller);
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("Успех")
                    .AddText("Ожидайте подтверждения регистрации вашего аккаунта от администратора")
                    .Show();
                _mainViewModel.SetLogin.Execute(null);
            }
        }

        public void OnDayChanged(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}