using EmployeeManagementSystem.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmployeeManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ---------------- EXISTING TABLES (UNCHANGED) ----------------

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // ---------------- STEP 2 : GEO-FENCING TABLES ----------------

        public DbSet<GeoFence> GeoFences { get; set; }
        public DbSet<EmployeeLocation> EmployeeLocations { get; set; }

        // ---------------- MODEL CONFIG ----------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ FIX: DateOnly <-> DateTime conversion (UNCHANGED)
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d)
            );

            modelBuilder.Entity<Leave>()
                .Property(l => l.LeaveDate)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");
        }
    }
}
