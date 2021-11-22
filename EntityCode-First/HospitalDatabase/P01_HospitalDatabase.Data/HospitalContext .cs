using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;
using System;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }

        public HospitalContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.Connection);
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<PatientMedicament> PatientMedicaments{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity
                .Property(p => p.FirstName)
                .HasMaxLength(50)
                .IsUnicode(true);

                entity
                .Property(p => p.LastName)
                .HasMaxLength(50)
                .IsUnicode(true);

                entity
                .Property(p => p.Address)
                .HasMaxLength(250)
                .IsUnicode(true);

                entity
                .Property(p => p.Email)
                .HasMaxLength(80)
                .IsUnicode(false);
            });

            modelBuilder.Entity<Visitation>(entity =>
            {
                entity
                .Property(v => v.Comments)
                .HasMaxLength(250)
                .IsUnicode(true);

                entity
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visitations)
                .HasForeignKey(v => v.PatientId);
            });

            modelBuilder.Entity<Diagnose>(entity =>
            {
                entity
                .Property(d => d.Name)
                .HasMaxLength(50)
                .IsUnicode(true);

                entity
                .Property(d => d.Comments)
                .HasMaxLength(250)
                .IsUnicode(true);

                entity
                .HasOne(d => d.Patient)
                .WithMany(p => p.Diagnoses)
                .HasForeignKey(d => d.PatientId);
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity
                .Property(m => m.Name)
                .HasMaxLength(50)
                .IsUnicode(true);
            });

            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(pm => new { pm.PatientId, pm.MedicamentId });

                entity
                .HasOne(p => p.Medicament)
                .WithMany(m => m.Prescriptions);

                entity
                .HasOne(m => m.Patient)
                .WithMany(p => p.Prescriptions);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity
                .Property(d => d.Name)
                .HasMaxLength(100)
                .IsUnicode(true);

            });
        }
    }
}
