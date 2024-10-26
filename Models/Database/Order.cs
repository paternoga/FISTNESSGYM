using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("Order", Schema = "dbo")]
    public partial class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public AspNetUser AspNetUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public int OrderStatusId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string OrderStatusName { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}