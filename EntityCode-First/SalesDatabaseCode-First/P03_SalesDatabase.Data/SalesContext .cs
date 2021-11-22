using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;
using System;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext()
        {

        }

        public SalesContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.Connection);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(s => s.Date).HasDefaultValueSql("GETDATE()");

                entity
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales);

                entity
                .HasOne(s => s.Product)
                .WithMany(p => p.Sales);

                entity
                .HasOne(s => s.Store)
                .WithMany(st => st.Sales);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsUnicode(true);

                entity.Property(c => c.Email).HasMaxLength(80).IsUnicode(false);

            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name).HasMaxLength(50).IsUnicode(true);

                entity
                .Property(p => p.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No description");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(s => s.Name).HasMaxLength(80).IsUnicode(true);
            });
        }
    }
}
