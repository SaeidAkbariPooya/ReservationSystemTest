using Microsoft.EntityFrameworkCore;
using ReservationSystem.Core.Entities;

namespace ReservationSystem.Infra.Context
{
    public class ReservationSystemDbContext : DbContext
    {
        public ReservationSystemDbContext(DbContextOptions<ReservationSystemDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تنظیم روابط و ایندکس‌ها
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Resource)
                .WithMany(res => res.Reservations)
                .HasForeignKey(r => r.ResourceId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId);

            // ایندکس برای بهبود جستجوهای تداخل زمانی
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.ResourceId, r.StartTime, r.EndTime, r.Status });
        }
    }
}
