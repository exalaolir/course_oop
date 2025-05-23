﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using course_oop;

#nullable disable

namespace course_oop.Migrations
{
    [DbContext(typeof(AppContext))]
    [Migration("20250521224054_Order22")]
    partial class Order22
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("course_oop.Core.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("course_oop.Core.Entities.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ShopId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CourierId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeliveryAddress")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ShopId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<double?>("X")
                        .HasColumnType("float");

                    b.Property<double?>("Y")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("CategotyId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Mark")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ShopId")
                        .HasColumnType("int");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ShopId");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("course_oop.Core.Entities.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages", (string)null);
                });

            modelBuilder.Entity("course_oop.Core.Entities.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<int?>("ShopId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("ShopId");

                    b.ToTable("Rewiews");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Shop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Adress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SallerId")
                        .HasColumnType("int");

                    b.Property<double?>("X")
                        .HasColumnType("float");

                    b.Property<double?>("Y")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("SallerId");

                    b.ToTable("Shops", (string)null);
                });

            modelBuilder.Entity("course_oop.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Banned")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Role").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("course_oop.Core.Entities.Admin", b =>
                {
                    b.HasBaseType("course_oop.Core.Entities.User");

                    b.HasDiscriminator().HasValue("Admin");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Courier", b =>
                {
                    b.HasBaseType("course_oop.Core.Entities.User");

                    b.Property<string>("Adress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("CurrentWeight")
                        .HasColumnType("float");

                    b.Property<bool?>("IsWork")
                        .HasColumnType("bit");

                    b.Property<int?>("SallerMinutes")
                        .HasColumnType("int");

                    b.Property<string>("StatusMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Transport")
                        .HasColumnType("int");

                    b.Property<int?>("UserMinutes")
                        .HasColumnType("int");

                    b.Property<double?>("Veight")
                        .HasColumnType("float");

                    b.Property<double?>("X")
                        .HasColumnType("float");

                    b.Property<double?>("Y")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("Courier");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Saller", b =>
                {
                    b.HasBaseType("course_oop.Core.Entities.User");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("SalersId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Vished")
                        .HasColumnType("bit");

                    b.HasIndex("CategoryId");

                    b.HasDiscriminator().HasValue("Saler");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Category", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Category", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Image", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Shop", "Shop")
                        .WithMany("Images")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Order", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Product");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Product", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("course_oop.Core.Entities.Shop", "Shop")
                        .WithMany("Products")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("course_oop.Core.Entities.ProductImage", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Review", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Product", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("course_oop.Core.Entities.Shop", "Shop")
                        .WithMany("Reviews")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Product");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Shop", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Saller", "Saller")
                        .WithMany("Shops")
                        .HasForeignKey("SallerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Saller");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Saller", b =>
                {
                    b.HasOne("course_oop.Core.Entities.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Category");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Category", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Product", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Shop", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Products");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("course_oop.Core.Entities.Saller", b =>
                {
                    b.Navigation("Shops");
                });
#pragma warning restore 612, 618
        }
    }
}
