using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("SubscriptionStatus", Schema = "dbo")]
    public partial class SubscriptionStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Status { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }
    }
}