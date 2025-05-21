using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course_oop.Core.Entities
{
    public class Saller : User
    {
        public string? SalersId { get; set; }

        public bool Vished { get; set; } = false;

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public virtual ICollection<Shop>? Shops { get; set; } = new List<Shop>();

        public Saller()
        {
        }

        public Saller(User user)
        {
            Id = user.Id;
            Email = user.Email;
            Password = user.Password;
            Phone = user.Phone;
            Name = user.Name;
            FirstName = user.FirstName;
            Banned = user.Banned;

            SalersId = null;
            Vished = false;
            Category = null;
            Shops = null;
        }

        public Saller(Courier user)
        {
            Id = user.Id;
            Email = user.Email;
            Password = user.Password;
            Phone = user.Phone;
            Name = user.Name;
            FirstName = user.FirstName;
            Banned = user.Banned;

            SalersId = null;
            Vished = false;
            Category = null;
            Shops = null;
        }

        public Saller(Admin user)
        {
            Id = user.Id;
            Email = user.Email;
            Password = user.Password;
            Phone = user.Phone;
            Name = user.Name;
            FirstName = user.FirstName;
            Banned = user.Banned;

            SalersId = null;
            Vished = false;
            Category = null;
            Shops = null;
        }
    }
}