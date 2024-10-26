using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("AspNetUserTokens", Schema = "dbo")]
    public partial class AspNetUserToken
    {
        [Key]
        [Required]
        public string UserId { get; set; }

        public AspNetUser AspNetUser { get; set; }

        [Key]
        [Required]
        public string LoginProvider { get; set; }

        [Key]
        [Required]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}