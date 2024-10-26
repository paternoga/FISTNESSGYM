using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("Reservation", Schema = "dbo")]
    public partial class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public AspNetUser AspNetUser { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}