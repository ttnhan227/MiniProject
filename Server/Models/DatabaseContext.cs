using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AccountModel> Accounts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure primary keys
    modelBuilder.Entity<AccountModel>()
        .HasKey(a => a.Username);

    modelBuilder.Entity<Product>()
        .HasKey(p => p.Id);

    modelBuilder.Entity<Cart>()
        .HasKey(c => new { c.UserName, c.ProductId });

    // Configure relationships
    modelBuilder.Entity<Cart>()
        .HasOne(c => c.AccountModel)
        .WithMany()
        .HasForeignKey(c => c.UserName)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Cart>()
        .HasOne(c => c.Product)
        .WithMany(p => p.Carts)
        .HasForeignKey(c => c.ProductId)
        .HasPrincipalKey(p => p.Id)
        .OnDelete(DeleteBehavior.Cascade);

    // Configure property constraints
    modelBuilder.Entity<AccountModel>()
        .Property(a => a.Role)
        .IsRequired();

    modelBuilder.Entity<Product>()
        .Property(p => p.Name)
        .IsRequired()
        .HasMaxLength(100);

    modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .IsRequired();

    // Seed data for AccountModel
    modelBuilder.Entity<AccountModel>().HasData(
        new AccountModel
        {
            Username = "admin",
            Password = "admin123", // Ideally, passwords should be hashed
            Role = "admin",
            Fullname = "Admin User",
            TotalAmount = 0
        },
        new AccountModel
        {
            Username = "user1",
            Password = "user123", // Ideally, passwords should be hashed
            Role = "user",
            Fullname = "User One",
            TotalAmount = 100
        }
    );

    // Seed data for Product
    modelBuilder.Entity<Product>().HasData(
        new Product
        {
            Id = 1,
            Name = "Product A",
            Price = 50,
            Description = "Description for Product A"
        },
        new Product
        {
            Id = 2,
            Name = "Product B",
            Price = 30,
            Description = "Description for Product B"
        }
    );

    // Seed data for Cart
    modelBuilder.Entity<Cart>().HasData(
        new Cart
        {
            UserName = "user1",
            ProductId = 1,
            Quantity = 2
        }
    );

    base.OnModelCreating(modelBuilder);
}

}