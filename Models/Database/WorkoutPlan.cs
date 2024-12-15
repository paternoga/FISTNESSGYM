using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("WorkoutPlan", Schema = "dbo")]
    public partial class WorkoutPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string UserId { get; set; }

        public AspNetUser AspNetUser { get; set; }

        public DateTime CreatedDate { get; set; }

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
        public string InstructorEmail { get; set; }
    }
}