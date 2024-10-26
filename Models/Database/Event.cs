using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("Event", Schema = "dbo")]
    public partial class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string EventName { get; set; }

        public string InstructorName { get; set; }

        [Required]
        public DateTime EventStartDate { get; set; }

        [Required]
        public DateTime EventEndDate { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        [Required]
        public int Participants { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}