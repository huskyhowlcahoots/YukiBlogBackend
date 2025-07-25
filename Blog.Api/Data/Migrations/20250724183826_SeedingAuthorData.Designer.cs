﻿// <auto-generated />
using Blog.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Blog.Api.Data.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("20250724183826_SeedingAuthorData")]
    partial class SeedingAuthorData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.7");

            modelBuilder.Entity("Blog.Api.DbEntities.Author.AuthorEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Authors");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "William",
                            Surname = "Shakespeare"
                        },
                        new
                        {
                            Id = 2,
                            Name = "J.K",
                            Surname = "Rowling"
                        },
                        new
                        {
                            Id = 3,
                            Name = "George R.R",
                            Surname = "Martin"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Charles",
                            Surname = "Dickens"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Stephen",
                            Surname = "King"
                        });
                });

            modelBuilder.Entity("Blog.Api.DbEntities.Post.PostEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Blog.Api.DbEntities.Post.PostEntity", b =>
                {
                    b.HasOne("Blog.Api.DbEntities.Author.AuthorEntity", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });
#pragma warning restore 612, 618
        }
    }
}
