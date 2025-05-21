using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using course_oop.Core.Interfaces;

namespace course_oop.Core.Entities
{
    public class Image : IImage
    {
        public int Id { get; set; }

        public required string Path { get; set; }

        public int ShopId { get; set; }
        public virtual Shop? Shop { get; set; }
    }
}