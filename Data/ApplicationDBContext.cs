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
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(acc => acc.Role).HasConversion<string>();
            modelBuilder.Entity<Staff>().Property(st => st.Gender).HasConversion<string>();
            modelBuilder.Entity<Admin>().Property(ad => ad.Gender).HasConversion<string>();
        }
    }
}
