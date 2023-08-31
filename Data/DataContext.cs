using Agriculture.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Agriculture.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer("Server=LAPTOP-M5RV82AT;Database=ProjectReactN;Trusted_Connection=True;TrustServerCertificate=True;");
            optionsBuilder.UseSqlServer("Server=10.103.0.30,1433;Database=AgricultureDB;user id=student;password=Com@2022;MultipleActiveResultSets=True;Trusted_Connection=False;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TransferPoint>()
       .HasOne(t => t.Users)
       .WithMany()
       .HasForeignKey(t => t.UsersId)
       .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TransferPoint>()
                .HasOne(t => t.ToUsers)
                .WithMany()
                .HasForeignKey(t => t.ToUsersId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Coupons> Coupons { get; set; }
        public DbSet<CouponUser> CouponUsers { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<TransferPoint> TransferPoints { get; set; }
        public DbSet<PointSettings> PointSettings { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<Shipping> Shippings { get; set; }
    }
}
