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

            modelBuilder.Entity("StatusExposed.Models.StatusInformation", b =>
                {
                    b.Property<string>("ServicePageDomain")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("Ping")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StatusPageUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("ServicePageDomain");

                    b.ToTable("Services");
                });
#pragma warning restore 612, 618
        }
    }
}