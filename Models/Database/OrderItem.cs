using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("OrderItem", Schema = "dbo")]
    public partial class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
    }
}