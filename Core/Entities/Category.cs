using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course_oop.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required] public string? Name { get; set; }

        public int? ParentId { get; set; }


        public Category? Parent { get; set; }

        public ICollection<Category> Children { get; set; } = new List<Category>();
    }
}