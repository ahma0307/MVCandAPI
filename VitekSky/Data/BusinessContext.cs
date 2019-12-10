using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitekSky.Models;

namespace VitekSky.Data
{
    public class BusinessContext : DbContext
    {
        public BusinessContext(DbContextOptions<BusinessContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<ProductGuide> ProductGuides { get; set; }
        public DbSet<CountryAssignment> CountryAssignments { get; set; }
        public DbSet<ProductAssignment> ProductAssignments { get; set; }
        public DbSet<Person> People { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Market>().ToTable("Market");
            modelBuilder.Entity<ProductGuide>().ToTable("ProductGuide");
            modelBuilder.Entity<CountryAssignment>().ToTable("CountryAssignment");
            modelBuilder.Entity<ProductAssignment>().ToTable("ProductAssignment");
            modelBuilder.Entity<Person>().ToTable("Person");


            modelBuilder.Entity<ProductAssignment>()
                .HasKey(p => new { p.ProductID, p.ProductGuideID });
        }
        //public DbSet<VitekSky.Models.Customer> Customer { get; set; }
    }
}
