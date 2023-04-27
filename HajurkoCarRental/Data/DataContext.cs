using HajurkoCarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace HajurkoCarRental.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<CarRental> CarRental { get; set; }
        public DbSet<CarRentalRequest> CarRentalRequest { get; set; }
        public DbSet<CarDamage> CarDamages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarRental>()
                .HasOne(c => c.Staff)
                .WithMany()
                .HasForeignKey(c => c.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CarRental>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
