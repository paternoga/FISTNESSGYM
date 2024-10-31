using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("Subscription", Schema = "dbo")]
    public partial class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public AspNetUser AspNetUser { get; set; }

        [Required]
        public int SubscriptionTypeId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public int SubscriptionStatusId { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; }
        [Required]
        public decimal Price { get; set; }

        public virtual SubscriptionType SubscriptionType { get; set; }
    }   
}