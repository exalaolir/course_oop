using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Registration;

namespace course_oop.Presentation.ViewModels.UsersPart
{
    internal sealed class InfoViewModel : ValidatedViewModel, INavigated, IRepeatPassword, IPassword
    {
        public INavigation? Navigator { get; set; }

        private string _firstName;
        private string _name;
        private string _email;
        private string _phone;
        private string _pasword;
        private string _repitedPassword;


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

        private  User _user;
        private UsersMainViewModelcs _vm;
        public InfoViewModel (ref User user, UsersMainViewModelcs vm)
        {
            _firstName = user.FirstName;
            _name = user.Name;
            _email = user.Email;
            _phone = user.Phone;
            _vm = vm;
            _user = user;

            _validFields = new bool[6];
            Array.Fill(_validFields, true);
            _validFields[4] = false;
            _validFields[5] = false;
        }

        public ICommand Change => new Command(() =>
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

            var validResults = ValidationServis.ValidateUser(user, _user);

            foreach (var result in validResults)
            {
                SetValidationResults(result.result, result.property, result.message ?? []);
            }

            if (!HasErrors)
            {
                using AppContext appContext = new AppContext();

                var newuser = appContext.Users.Find(_user.Id);

                newuser.Name = user.Name;
                newuser.Email = user.Email; 
                newuser.Phone = user.Phone;
                newuser.FirstName = user.FirstName;
                newuser.Password = user.Password;

                appContext.SaveChanges();
                _vm._user = newuser;
            }
        });
    }
}