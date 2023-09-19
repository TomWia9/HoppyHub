﻿// <auto-generated />
using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BreweryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("PostCode")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("BreweryId")
                        .IsUnique();

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Domain.Entities.Beer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("AlcoholByVolume")
                        .HasColumnType("float");

                    b.Property<Guid>("BeerStyleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Blg")
                        .HasColumnType("float");

                    b.Property<Guid>("BreweryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Composition")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime?>("Created")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedBy")
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(3000)
                        .HasColumnType("nvarchar(3000)");

                    b.Property<int?>("Ibu")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModified")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("BeerStyleId");

                    b.HasIndex("BreweryId");

                    b.ToTable("Beers");
                });

            modelBuilder.Entity("Domain.Entities.BeerImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BeerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TempImage")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("BeerId")
                        .IsUnique();

                    b.ToTable("BeerImages");
                });

            modelBuilder.Entity("Domain.Entities.BeerStyle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CountryOfOrigin")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("Created")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedBy")
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime?>("LastModified")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("BeerStyles");
                });

            modelBuilder.Entity("Domain.Entities.Brewery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Created")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedBy")
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FoundationYear")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModified")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("WebsiteUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Breweries");
                });

            modelBuilder.Entity("Domain.Entities.Favorite", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BeerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastModified")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Domain.Entities.Opinion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BeerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageUri")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("LastModified")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.ToTable("Opinions");
                });

            modelBuilder.Entity("Domain.Entities.Address", b =>
                {
                    b.HasOne("Domain.Entities.Brewery", "Brewery")
                        .WithOne("Address")
                        .HasForeignKey("Domain.Entities.Address", "BreweryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brewery");
                });

            modelBuilder.Entity("Domain.Entities.Beer", b =>
                {
                    b.HasOne("Domain.Entities.BeerStyle", "BeerStyle")
                        .WithMany("Beers")
                        .HasForeignKey("BeerStyleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Brewery", "Brewery")
                        .WithMany("Beers")
                        .HasForeignKey("BreweryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BeerStyle");

                    b.Navigation("Brewery");
                });

            modelBuilder.Entity("Domain.Entities.BeerImage", b =>
                {
                    b.HasOne("Domain.Entities.Beer", "Beer")
                        .WithOne("BeerImage")
                        .HasForeignKey("Domain.Entities.BeerImage", "BeerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Beer");
                });

            modelBuilder.Entity("Domain.Entities.Favorite", b =>
                {
                    b.HasOne("Domain.Entities.Beer", "Beer")
                        .WithMany("Favorites")
                        .HasForeignKey("BeerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Beer");
                });

            modelBuilder.Entity("Domain.Entities.Opinion", b =>
                {
                    b.HasOne("Domain.Entities.Beer", "Beer")
                        .WithMany("Opinions")
                        .HasForeignKey("BeerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Beer");
                });

            modelBuilder.Entity("Domain.Entities.Beer", b =>
                {
                    b.Navigation("BeerImage");

                    b.Navigation("Favorites");

                    b.Navigation("Opinions");
                });

            modelBuilder.Entity("Domain.Entities.BeerStyle", b =>
                {
                    b.Navigation("Beers");
                });

            modelBuilder.Entity("Domain.Entities.Brewery", b =>
                {
                    b.Navigation("Address");

                    b.Navigation("Beers");
                });
#pragma warning restore 612, 618
        }
    }
}
