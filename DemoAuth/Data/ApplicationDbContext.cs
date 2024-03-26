using DemoAuth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoAuth.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed static data
            modelBuilder.Entity<Product>().HasData(
                  new Product { Id = 1, Name = "Product 1", Price = 1, CategoryId = 1 },
                  new Product { Id = 2, Name = "Product 2", Price = 2, CategoryId = 2 },
                  new Product { Id = 3, Name = "Product 3", Price = 3, CategoryId = 3 },
                  new Product { Id = 4, Name = "Product 4", Price = 4, CategoryId = 3 },
                  new Product { Id = 5, Name = "Product 5", Price = 5, CategoryId = 2 },
                  new Product { Id = 6, Name = "Product 6", Price = 6, CategoryId = 1 },
                  new Product { Id = 7, Name = "Product 7", Price = 7, CategoryId = 1 },
                  new Product { Id = 8, Name = "Product 8", Price = 8, CategoryId = 2 },
                  new Product { Id = 9, Name = "Product 9", Price = 9, CategoryId = 3 },
                  new Product { Id = 10, Name = "Product 10", Price = 10, CategoryId = 3 },
                  new Product { Id = 11, Name = "Product 11", Price = 11, CategoryId = 2 }
              );

            modelBuilder.Entity<Category>().HasData(
                  new Category { Id = 1, Name = "Electronics" },
                  new Category { Id = 2, Name = "Clothing" },
                  new Category { Id = 3, Name = "Produce" }
              );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableDetailedErrors();  // Enable detailed error messages
                                          // .EnableSensitiveDataLogging()
                                          // .LogTo(Console.WriteLine);
        }

    }
}
