﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tv7Playlist.Data;

namespace Tv7Playlist.Data.Migrations
{
    [DbContext(typeof(PlaylistContext))]
    [Migration("20200206214445_DotnetCore3_1_upgrade")]
    partial class DotnetCore3_1_upgrade
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("Tv7Playlist.Data.PlaylistEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("ChannelNumberExport")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChannelNumberImport")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("EpgMatchName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UrlOriginal")
                        .HasColumnType("TEXT");

                    b.Property<string>("UrlProxy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChannelNumberImport")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.ToTable("PlaylistEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
