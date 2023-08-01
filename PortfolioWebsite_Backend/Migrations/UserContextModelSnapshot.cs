﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PortfolioWebsite_Backend.Models.UserModel;

#nullable disable

namespace PortfolioWebsite_Backend.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PortfolioWebsite_Backend.Models.UserModel.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AccessToken = "",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "coleman399@gmail.com",
                            PasswordHash = "$2a$11$bev1froRw6iWdFWN2WhOeuUNCa.OgPsnHL2h0Xf/GgpNUFbtHO6tu",
                            Role = "SuperUser",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "coleman399"
                        });
                });

            modelBuilder.Entity("PortfolioWebsite_Backend.Models.UserModel.User", b =>
                {
                    b.OwnsOne("PortfolioWebsite_Backend.Models.UserModel.ForgotPasswordToken", "ForgotPasswordToken", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("ExpiresAt")
                                .HasColumnType("datetime2");

                            b1.Property<bool>("IsValidated")
                                .HasColumnType("bit");

                            b1.Property<string>("Token")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("UserId")
                                .HasColumnType("int");

                            b1.Property<int?>("UserId1")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("UserId")
                                .IsUnique();

                            b1.HasIndex("UserId1");

                            b1.ToTable("ForgotPasswordTokens");

                            b1.WithOwner()
                                .HasForeignKey("UserId");

                            b1.HasOne("PortfolioWebsite_Backend.Models.UserModel.User", "User")
                                .WithMany()
                                .HasForeignKey("UserId1");

                            b1.Navigation("User");
                        });

                    b.OwnsOne("PortfolioWebsite_Backend.Models.UserModel.RefreshToken", "RefreshToken", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("ExpiresAt")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Token")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("UserId")
                                .HasColumnType("int");

                            b1.Property<int?>("UserId1")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("UserId")
                                .IsUnique();

                            b1.HasIndex("UserId1");

                            b1.ToTable("RefreshTokens");

                            b1.WithOwner()
                                .HasForeignKey("UserId");

                            b1.HasOne("PortfolioWebsite_Backend.Models.UserModel.User", "User")
                                .WithMany()
                                .HasForeignKey("UserId1");

                            b1.Navigation("User");
                        });

                    b.Navigation("ForgotPasswordToken");

                    b.Navigation("RefreshToken");
                });
#pragma warning restore 612, 618
        }
    }
}
