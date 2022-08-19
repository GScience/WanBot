﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WanBot.Plugin.Essential.EssAttribute;

#nullable disable

namespace WanBot.Plugin.Essential.Migrations
{
    [DbContext(typeof(EssAttributeDatabaseContext))]
    [Migration("20220819153046_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("WanBot.Plugin.Essential.EssAttribute.DbEssAttributeUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AttactAddition")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DefenceAddition")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Energy")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EnergyMax")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HpAddition")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastTimeCheckEnergy")
                        .HasColumnType("TEXT");

                    b.Property<int>("MagicAddition")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Money")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SpAttactAddition")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SpDefenceAddition")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
