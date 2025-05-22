using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course_oop.Core.Entities
{
    public enum OrderStatus
    {
        [Display(Name = "В корзине")] InCart,

        [Display(Name = "В обработке")] Processing,

        [Display(Name = "Готов к выдаче курьеру")]
        ReadyForCourier,

        [Display(Name = "Ожидает курьера")]
        WaitCourier,

        [Display(Name = "Доставляется")] InDelivery,

        [Display(Name = "Доставлен")] Delivered,

        [Display(Name = "Отклонён")] Rejected
    }

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; } = OrderStatus.InCart;


        [StringLength(500)] public string? DeliveryAddress { get; set; }

        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }

        [Required] public required int UserId { get; set; }


        [Required] public int ShopId { get; set; }

        public double? X { get; set; }
        public double? Y { get; set; }

        
        public required string Name { get; set; }

        public required decimal Price { get; set; }
      
        public int? CourierId { get; set; }
    }
}