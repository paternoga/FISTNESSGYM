using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("Measurement", Schema = "dbo")]
    public partial class Measurement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }  
        public virtual ApplicationUser User { get; set; }  

        public decimal Weight { get; set; }
        public decimal WaistCircumference { get; set; }
        public decimal ChestCircumference { get; set; }
        public decimal ArmCircumference { get; set; }
        public decimal LegCircumference { get; set; }
        public decimal HipCircumference { get; set; }
        public decimal BodyFat { get; set; }
        public DateTime MeasurementDate { get; set; }
        public string Notes { get; set; }
    }

}
