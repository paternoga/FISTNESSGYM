using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Data
{
    public partial class databaseContext : DbContext
    {
        public databaseContext()
        {
        }

        public databaseContext(DbContextOptions<databaseContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserLogin>().HasKey(table => new {
                table.LoginProvider, table.ProviderKey
            });

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserRole>().HasKey(table => new {
                table.UserId, table.RoleId
            });

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserToken>().HasKey(table => new {
                table.UserId, table.LoginProvider, table.Name
            });

            builder.Entity<FISTNESSGYM.Models.database.Order>()
              .HasOne(i => i.OrderStatus)
              .WithMany(i => i.Orders)
              .HasForeignKey(i => i.OrderStatusId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.Order>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.Orders)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.OrderItem>()
              .HasOne(i => i.Order)
              .WithMany(i => i.OrderItems)
              .HasForeignKey(i => i.OrderId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.OrderItem>()
              .HasOne(i => i.Product)
              .WithMany(i => i.OrderItems)
              .HasForeignKey(i => i.ProductId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.Reservation>()
              .HasOne(i => i.Event)
              .WithMany(i => i.Reservations)
              .HasForeignKey(i => i.EventId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.Reservation>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.Reservations)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.Subscription>()
              .HasOne(i => i.SubscriptionStatus)
              .WithMany(i => i.Subscriptions)
              .HasForeignKey(i => i.SubscriptionStatusId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.Subscription>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.Subscriptions)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.AspNetRoleClaim>()
              .HasOne(i => i.AspNetRole)
              .WithMany(i => i.AspNetRoleClaims)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserClaim>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserClaims)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserLogin>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserLogins)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserRole>()
              .HasOne(i => i.AspNetRole)
              .WithMany(i => i.AspNetUserRoles)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserRole>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserRoles)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.AspNetUserToken>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserTokens)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<FISTNESSGYM.Models.database.Event>()
              .Property(p => p.EventStartDate)
              .HasColumnType("datetime");

            builder.Entity<FISTNESSGYM.Models.database.Event>()
              .Property(p => p.EventEndDate)
              .HasColumnType("datetime");

            builder.Entity<FISTNESSGYM.Models.database.Order>()
              .Property(p => p.OrderDate)
              .HasColumnType("datetime");

            builder.Entity<FISTNESSGYM.Models.database.Subscription>()
              .Property(p => p.StartDate)
              .HasColumnType("datetime");

            builder.Entity<FISTNESSGYM.Models.database.Subscription>()
              .Property(p => p.EndDate)
              .HasColumnType("datetime");

            builder.Entity<FISTNESSGYM.Models.database.AspNetUser>()
              .Property(p => p.LockoutEnd)
              .HasColumnType("datetimeoffset");
            this.OnModelBuilding(builder);
        }

        public DbSet<FISTNESSGYM.Models.database.Event> Events { get; set; }

        public DbSet<FISTNESSGYM.Models.database.Order> Orders { get; set; }

        public DbSet<FISTNESSGYM.Models.database.OrderItem> OrderItems { get; set; }

        public DbSet<FISTNESSGYM.Models.database.OrderStatus> OrderStatuses { get; set; }

        public DbSet<FISTNESSGYM.Models.database.Product> Products { get; set; }

        public DbSet<FISTNESSGYM.Models.database.Reservation> Reservations { get; set; }

        public DbSet<FISTNESSGYM.Models.database.Subscription> Subscriptions { get; set; }

        public DbSet<FISTNESSGYM.Models.database.SubscriptionStatus> SubscriptionStatuses { get; set; }

        public DbSet<FISTNESSGYM.Models.database.AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public DbSet<FISTNESSGYM.Models.database.AspNetRole> AspNetRoles { get; set; }

        public DbSet<FISTNESSGYM.Models.database.AspNetUserClaim> AspNetUserClaims { get; set; }

        public DbSet<FISTNESSGYM.Models.database.AspNetUserLogin> AspNetUserLogins { get; set; }

        public DbSet<FISTNESSGYM.Models.database.AspNetUserRole> AspNetUserRoles { get; set; }

        public DbSet<FISTNESSGYM.Models.database.AspNetUser> AspNetUsers { get; set; }

        public DbSet<FISTNESSGYM.Models.database.AspNetUserToken> AspNetUserTokens { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}