using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using course_oop.Presentation.Views.User;

namespace course_oop.Core.Entities
{
    public class Shop
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int SallerId { get; set; }
        public virtual Saller? Saller { get; set; }

        public string? Description { get; set; }

        public double? X { get; set; }
        public double? Y { get; set; }

        public string? Adress { get; set; }
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}