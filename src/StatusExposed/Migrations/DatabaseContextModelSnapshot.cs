﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StatusExposed.Database;

#nullable disable

namespace StatusExposed.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.6");

            modelBuilder.Entity("StatusExposed.Models.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("StatusExposed.Models.StatusHistoryData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("Ping")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StatusInformationServicePageDomain")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StatusInformationServicePageDomain");

                    b.ToTable("StatusHistoryData");
                });

            modelBuilder.Entity("StatusExposed.Models.StatusInformation", b =>
                {
                    b.Property<string>("ServicePageDomain")
                        .HasColumnType("TEXT");

                    b.Property<string>("StatusPageUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("ServicePageDomain");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("StatusExposed.Models.Subscriber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StatusInformationServicePageDomain")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StatusInformationServicePageDomain");

                    b.ToTable("Subscriber");
                });

            modelBuilder.Entity("StatusExposed.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("SessionToken")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StatusExposed.Models.Permission", b =>
                {
                    b.HasOne("StatusExposed.Models.User", null)
                        .WithMany("Permissions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("StatusExposed.Models.StatusHistoryData", b =>
                {
                    b.HasOne("StatusExposed.Models.StatusInformation", null)
                        .WithMany("StatusHistory")
                        .HasForeignKey("StatusInformationServicePageDomain");
                });

            modelBuilder.Entity("StatusExposed.Models.Subscriber", b =>
                {
                    b.HasOne("StatusExposed.Models.StatusInformation", null)
                        .WithMany("Subscribers")
                        .HasForeignKey("StatusInformationServicePageDomain");
                });

            modelBuilder.Entity("StatusExposed.Models.StatusInformation", b =>
                {
                    b.Navigation("StatusHistory");

                    b.Navigation("Subscribers");
                });

            modelBuilder.Entity("StatusExposed.Models.User", b =>
                {
                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}
