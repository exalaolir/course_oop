using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using course_oop.Core.Interfaces;

namespace course_oop.Core.Entities
{
    public class ProductImage : IImage
    {
        public int Id { get; set; }

        public required string Path { get; set; }

        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}