using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("SubscriptionType", Schema = "dbo")]
    public class SubscriptionType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
    }
}
