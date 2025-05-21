using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Registration;
using course_oop.Shared.Const;
using Microsoft.Toolkit.Uwp.Notifications;

namespace course_oop.Presentation.ViewModels.AdminPart
{
    internal class RegisterCourierViewModel : ValidatedViewModel, IPassword, IRepeatPassword
    {
        private string _firstName;
        private string _name;
        private string _email;
        private string _phone;
        private string _pasword;
        private string _repitedPassword;
        private string _typeOfTransport;
        private double _veight;
        private readonly ValidationServis _validator;

        public RegisterCourierViewModel()
        {
            _firstName = string.Empty;
            _name = string.Empty;
            _email = string.Empty;
            _phone = string.Empty;
            _pasword = string.Empty;
            _repitedPassword = string.Empty;
            _typeOfTransport = string.Empty;

            _validFields = new bool[6];
            Register = new Command(OnRegister);
            _validator = new();
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


        public static ObservableCollection<string> Transport { get; } =
        [
            "Пешеход",
            "Велосипед",
            "Автомобиль",
        ];

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

        public ICommand Register { get; }

        public string TypeOfTransport
        {
            get => _typeOfTransport;
            set
            {
                this.SetValue(ref _typeOfTransport, value);
                switch (value)
                {
                    case "Пешеход":
                        _veight = 20;
                        break;
                    case "Велосипед":
                        _veight = 40;
                        break;
                    case "Автомобиль":
                        _veight = 200;
                        break;
                }
            }
        }

        private void OnRegister()
        {
            Courier courier = new()
            {
                Name = _name,
                FirstName = _firstName,
                Email = _email,
                Phone = _phone,
                Password = PasswordHasher.HashPassword(_pasword),
                Banned = false,
                Transport = _typeOfTransport switch
                {
                    "Пешеход" => Consts.Transport.Foot,
                    "Велосипед" => Consts.Transport.Bycke,
                    "Автомобиль" => Consts.Transport.Car,
                    _ => Consts.Transport.Foot
                },
                Veight = _veight == 0 ? 20 : _veight,
            };

            var validResults = _validator.ValidateUser((User)courier);

            foreach (var result in validResults)
            {
                SetValidationResults(result.result, result.property, result.message ?? []);
            }

            if (!HasErrors)
            {
                AuthentificationServies.AddCourier(courier);
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("Успех")
                    .AddText("Работник внесён в базу")
                    .Show();
            }
        }

        public void OnDayChanged(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}