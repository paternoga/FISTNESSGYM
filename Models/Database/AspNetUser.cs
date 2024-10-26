using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISTNESSGYM.Models.database
{
    [Table("AspNetUsers", Schema = "dbo")]
    public partial class AspNetUser
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public int AccessFailedCount { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public string NormalizedEmail { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string SecurityStamp { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string UserName { get; set; }

        public ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

        public ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

        public ICollection<AspNetUserRole> AspNetUserRoles { get; set; }

        public ICollection<AspNetUserToken> AspNetUserTokens { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }
    }
}