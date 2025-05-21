using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course_oop.Core.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required] public string Content { get; set; } = string.Empty;

        [Range(1, 5)] public int Rating { get; set; }

        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required] public string UserName { get; set; } = string.Empty; // Имя пользователя

        // Внешний ключ и навигационное свойство для Product (опционально)
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

        // Внешний ключ и навигационное свойство для Shop (опционально)
        public int? ShopId { get; set; }
        public virtual Shop? Shop { get; set; }
    }
}