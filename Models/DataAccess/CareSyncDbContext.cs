using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CareSync.Models.DataAccess
{
    public class CareSyncDbContext : DbContext
    {
        public CareSyncDbContext(DbContextOptions<CareSyncDbContext> options) : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>().HasKey(d => d.DoctorID);
            modelBuilder.Entity<Patient>().HasKey(p => p.PatientID);
            modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmentID);

            modelBuilder.Entity<Appointment>()
                .HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(a => a.DoctorID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne<Patient>()
                .WithMany()
                .HasForeignKey(a => a.PatientID)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
