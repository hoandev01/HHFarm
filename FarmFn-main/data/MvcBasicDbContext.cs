namespace mvcbasic.Data
{
    using Farm.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Data.SqlClient;

    public class MvcBasicDbContext : DbContext
    {
        public MvcBasicDbContext(DbContextOptions<MvcBasicDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Cage> Cage { get; set; }
        public DbSet<Animal> Animal { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Therapy> Therapy { get; set; }
        public DbSet<Deal> Deal { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        // thêm db
        public DbSet<Employee> Employees { get; set; }
        public DbSet<SurveyForm> SurveyForms { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne()
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);
        }
    }
}