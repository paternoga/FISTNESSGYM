using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("AspNetRoleClaims", Schema = "dbo")]
    public partial class AspNetRoleClaim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        [Required]
        public string RoleId { get; set; }

        public AspNetRole AspNetRole { get; set; }
    }
}