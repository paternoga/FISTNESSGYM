using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("Notifications", Schema = "dbo")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        public AspNetUser User { get; set; } 

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
