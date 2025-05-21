using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course_oop.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required] public required string Name { get; set; }

        public required string Description { get; set; }

        public double Mark { get; set; } = 0.0;

        [Required] public int Count { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Weight { get; set; }

        public int ShopId { get; set; }

        public virtual Shop? Shop { get; set; }

        public int CategotyId { get; set; }


        public virtual Category? Category { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}