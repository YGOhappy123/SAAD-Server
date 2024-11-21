using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using milktea_server.Models;

namespace milktea_server.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Milktea> Milkteas { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemTopping> OrderItemToppings { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartItemTopping> CartItemToppings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(acc => acc.Role).HasConversion<string>();
            modelBuilder.Entity<Admin>().Property(ad => ad.Gender).HasConversion<string>();
            modelBuilder.Entity<Order>().Property(od => od.Status).HasConversion<string>();
            modelBuilder.Entity<OrderItem>().Property(odi => odi.Size).HasConversion<string>();
            modelBuilder.Entity<CartItem>().Property(ci => ci.Size).HasConversion<string>();
        }
    }
}
