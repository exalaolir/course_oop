using System;
using System.Collections.Generic;
using System.IO;
using course_oop.Core.Entities;
using course_oop.Migrations;
using course_oop.Shared.Const;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace course_oop
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Saller> Sallers { get; set; }

        public DbSet<Courier> Couriers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Shop> Shops { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Core.Entities.Order> Orders { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Core.Entities.Review> Rewiews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Consts.DbConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<Consts.Roles>("Role")
                .HasValue<User>(Consts.Roles.User)
                .HasValue<Saller>(Consts.Roles.Saler)
                .HasValue<Admin>(Consts.Roles.Admin)
                .HasValue<Courier>(Consts.Roles.Courier);

            modelBuilder.Entity<Category>().ToTable("Category");

            modelBuilder.Entity<Saller>()
                .HasOne(e => e.Category)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Children)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .Property("Role")
                .HasConversion<string>();

            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Images)
                .WithOne(i => i.Shop)
                .HasForeignKey(i => i.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Shop>().ToTable("Shops");

            modelBuilder.Entity<Saller>()
                .HasMany(o => o.Shops)
                .WithOne(s => s.Saller)
                .HasForeignKey(s => s.SallerId);

            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Shop)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

         
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // или Restrict в зависимости от требований

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Shop)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ShopId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Core.Entities.Order>()
                .HasOne(o => o.Product)
                .WithMany() // Если у Product нет коллекции Orders
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.NoAction); // Или Cascade в зависимости от требований


            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<ProductImage>().ToTable("ProductImages");
            //modelBuilder.Entity<Review>().ToTable("Reviews");
        }
    }
}