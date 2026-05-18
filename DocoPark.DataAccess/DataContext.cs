using DocoPark.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocoPark.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<ParkingSpot> ParkingSpots { get; set; }

        public DbSet<ParkingSession> ParkingSessions { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }
    }


}
