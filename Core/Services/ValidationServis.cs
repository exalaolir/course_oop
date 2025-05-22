using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using course_oop.Core.Entities;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Shared.Const;
using ValidatorResult = (bool result, string property, System.Collections.Generic.IReadOnlyList<string>? message);

namespace course_oop.Core.Services
{
    internal sealed class ValidationServis()
    {
        internal static List<ValidatorResult> ValidateUser(User user, User? user2 = null)
        {
            using var _repo = new Repo();
            var users = _repo.GetAllUsers().Cast<User>()
                .Union(_repo.GetSealers().Cast<User>())
                .Union(_repo.GetCouriers().Cast<User>())
                .Union(_repo.GetAdmins().Cast<User>()).ToList();

            List<ValidatorResult> results = [];
            results.Capacity = Consts.ValidatorCapacity;

            if(user2 != null) 
                users = users.Where(u => u.Id != user2.Id).ToList();

            foreach (User registeredUser in users)
            {
                if (registeredUser.Email == user.Email)
                {
                    results.Add(
                        (result: false,
                            property: nameof(user.Email),
                            message: Consts.EmailMessage)
                    );
                }

                if (registeredUser.Phone == user.Phone)
                {
                    results.Add(
                        (result: false,
                            property: nameof(user.Phone),
                            message: Consts.PhoneMessage)
                    );
                }
            }

            return results;
        }

        internal static List<ValidatorResult> ValidateCategory(Category newCategory, string fieldName)
        {
            using var _repo = new Repo();
            var categories = _repo.GetCategories();

            List<ValidatorResult> results = [];
            results.Capacity = Consts.ValidatorCapacity;

            foreach (Category category in categories)
            {
                if (newCategory.Name == category.Name)
                {
                    results.Add(
                        (result: false,
                            property: fieldName,
                            message: Consts.CategoryMessage)
                    );

                    break;
                }
            }

            return results;
        }


        internal static List<ValidatorResult> ValidateShop(Shop shop)
        {
            using var _repo = new Repo();

            var shops = _repo.GetShops().Cast<Shop>().Where(e => e.Saller != shop.Saller);

            List<ValidatorResult> results = [];
            results.Capacity = Consts.ValidatorCapacity;

            foreach (Shop existedShop in shops)
            {
                if (existedShop.Adress == shop.Adress)
                {
                    results.Add(
                        (result: false,
                            property: nameof(shop.Adress),
                            message: Consts.EmailMessage)
                    );
                }

                if (existedShop.Name == shop.Name)
                {
                    results.Add(
                        (result: false,
                            property: nameof(shop.Name),
                            message: Consts.ShopNameMessageMessage)
                    );
                }
            }

            return results;
        }

        internal List<ValidatorResult> ValidateSaler(Saller saller)
        {
            using var _repo = new Repo();
            var users = _repo.GetAllUsers().Cast<User>()
                .Concat(_repo.GetCouriers().Cast<User>())
                .Concat(_repo.GetAdmins().Cast<User>())
                .Select(u => new Saller(u))
                .Concat(_repo.GetSealers().Cast<User>()).ToList();

            List<ValidatorResult> results = [];
            results.Capacity = Consts.ValidatorCapacity;

            foreach (Saller user in users)
            {
                Saller registeredSaller = user;
                if (registeredSaller.Email == saller.Email)
                {
                    results.Add(
                        (result: false,
                            property: nameof(saller.Email),
                            message: Consts.EmailMessage)
                    );
                }

                if (registeredSaller.Phone == saller.Phone)
                {
                    results.Add(
                        (result: false,
                            property: nameof(saller.Phone),
                            message: Consts.PhoneMessage)
                    );
                }

                if (registeredSaller.SalersId == saller.SalersId)
                {
                    results.Add(
                        (result: false,
                            property: nameof(saller.SalersId),
                            message: Consts.SallerIdMessage)
                    );
                }
            }

            return results;
        }

        internal static (bool, IReadOnlyList<string>?) ValidateProperty<T>(object property, T validatedObject,
            string propertyName)
        {
            if (validatedObject == null)
                throw new ArgumentNullException(nameof(validatedObject), "Передан  null");

            var context = new ValidationContext(validatedObject)
            {
                MemberName = propertyName
            };

            var validationResults = new List<ValidationResult>();


            bool noErrors = Validator.TryValidateProperty(property, context, validationResults);

            var errors = validationResults
                .Where(r => !string.IsNullOrEmpty(r.ErrorMessage))
                .Select(r => r.ErrorMessage!).ToList() as IReadOnlyList<string>;

            return (noErrors, errors);
        }
    }
}