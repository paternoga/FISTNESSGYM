using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("AspNetUserLogins", Schema = "dbo")]
    public partial class AspNetUserLogin
    {
        [Key]
        [Required]
        public string LoginProvider { get; set; }

        [Key]
        [Required]
        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }

        [Required]
        public string UserId { get; set; }

        public AspNetUser AspNetUser { get; set; }
    }
}