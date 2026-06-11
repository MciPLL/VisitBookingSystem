using Microsoft.EntityFrameworkCore;
using VisitBookingSystem.Models;

namespace VisitBookingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dane startowe
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@example.com" },
                new Patient { Id = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@example.com" }
            );
        }
    }
}
