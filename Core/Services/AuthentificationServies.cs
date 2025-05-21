using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using course_oop.Core.Entities;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Shared.Const;


namespace course_oop.Core.Services
{
    internal class AuthentificationServies()
    {
        internal void AddUser(User user)
        {
            using var _repository = new Repo();
            _repository.AddUser(user);
        }

        internal void AddSaller(Saller seller)
        {
            using var _repository = new Repo();
            _repository.AddSaller(seller);
        }

        internal static void AddCourier(Courier courier)
        {
            using var _repository = new Repo();
            _repository.AddCourier(courier);
        }


        internal Consts.AuthResults Authentificate(string email, string pass,
            out (Consts.Roles role, object user)? results)
        {
            using var _repository = new Repo();

            var searchResults = new List<object?>()
            {
                _repository.FindAdminByEmail(email),
                _repository.FindSallerByEmail(email),
                _repository.FindUserByEmail(email),
                _repository.FindCurierByEmail(email)
            };

            results = null;
            var error = Consts.AuthResults.EmailError;

            if (searchResults[0] != null && CheckPass(searchResults[0]!, out error))
            {
                results = (Consts.Roles.Admin, searchResults[0]!);
            }
            else if (searchResults[1] != null && CheckPass(searchResults[1]!, out error))
            {
                results = (Consts.Roles.Saler, searchResults[1]!);
            }
            else if (searchResults[3] != null && CheckPass(searchResults[3]!, out error))
            {
                results = (Consts.Roles.Courier, searchResults[3]!);
            }
            else if (searchResults[2] != null && CheckPass(searchResults[2]!, out error))
            {
                results = (Consts.Roles.User, searchResults[2]!);
            }
            else
            {
                results = null;
                return error;
            }

            bool CheckPass(object data, out Consts.AuthResults errorType)
            {
                var user = (User)data;
                if (PasswordHasher.VerifyPassword(pass, user.Password!))
                {
                    errorType = Consts.AuthResults.EmailError;
                    return true;
                }
                else
                {
                    errorType = Consts.AuthResults.PasswordError;
                    return false;
                }
            }

            return Consts.AuthResults.Success;
        }
    }
}