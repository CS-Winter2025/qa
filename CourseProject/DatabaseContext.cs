using CourseProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }
        public DbSet<Asset> Assets { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Organization)
                .WithMany(o => o.Employees)
                .HasForeignKey(e => e.OrganizationId);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Services)
                .WithMany(s => s.Employees);

            modelBuilder.Entity<EventSchedule>()
                .HasMany(es => es.Employees)
                .WithMany(e => e.EventSchedules)
                .UsingEntity<Dictionary<string, object>>(
                    "EventScheduleEmployee", // Junction table name
                    r => r.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"), // Foreign key for Employee
                    r => r.HasOne<EventSchedule>().WithMany().HasForeignKey("EventScheduleId"), // Foreign key for EventSchedule
                    r =>
                    {
                        // Seed data for the junction table
                        r.HasData(
                            new { EventScheduleId = 1, EmployeeId = 1 },
                            new { EventScheduleId = 1, EmployeeId = 2 }
                        );
                    }
                );

            modelBuilder.Entity<EventSchedule>()
                .HasOne(es => es.Service)
                .WithMany()
                .HasForeignKey(es => es.ServiceID);

            modelBuilder.Entity<Resident>()
                .HasMany(r => r.Services)
                .WithMany(s => s.Residents) 
                .UsingEntity(j => j.HasData(
                    new { ResidentsResidentId = 1, ServicesServiceID = 1 },
                    new { ResidentsResidentId = 2, ServicesServiceID = 2 }
                ));

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Resident)
                .WithMany()
                .HasForeignKey(i => i.ResidentID)
                .IsRequired();

            modelBuilder.Entity<Organization>().HasData(
                new Organization { OrganizationId = 1 },
                new Organization { OrganizationId = 2 }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeId = 1, Name = "Alice", JobTitle = "Manager", EmploymentType = "Full-Time", PayRate = 60000, OrganizationId = 1 },
                new Employee { EmployeeId = 2, Name = "Bob", JobTitle = "Developer", EmploymentType = "Full-Time", PayRate = 50000, OrganizationId = 1, ManagerId = 1 }
            );

            modelBuilder.Entity<Resident>().HasData(
                new Resident { ResidentId = 1, Name = "Charlie" },
                new Resident { ResidentId = 2, Name = "Diana" }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceID = 1, Type = "Cleaning", Rate = 50 },
                new Service { ServiceID = 2, Type = "Security", Rate = 100 }
            );

            modelBuilder.Entity<Invoice>().HasData(
                new Invoice { InvoiceID = 1, ResidentID = 1, Date = DateTime.UtcNow, AmountDue = 200, AmountPaid = 100 }
            );

            modelBuilder.Entity<EventSchedule>().HasData(
                new EventSchedule { EventScheduleId = 1, ServiceID = 1 }
            );
        }

    }
}
