using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("WorkoutExercise", Schema = "dbo")]
    public partial class WorkoutExercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ExerciseId { get; set; }

        public Exercise Exercise { get; set; }

        [Required]
        public int WorkoutPlanId { get; set; }

        public WorkoutPlan WorkoutPlan { get; set; }

        [Required]
        public int Sets { get; set; }

        [Required]
        public int Reps { get; set; }
        
        public decimal? Weights { get; set; }
    }
}