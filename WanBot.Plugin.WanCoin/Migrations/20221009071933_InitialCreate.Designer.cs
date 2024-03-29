﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WanBot.Plugin.WanCoin;

#nullable disable

namespace WanBot.Plugin.WanCoin.Migrations
{
    [DbContext(typeof(WanCoinDatabase))]
    [Migration("20221009071933_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("WanBot.Plugin.WanCoin.WanCoinHash", b =>
                {
                    b.Property<long>("Hash")
                        .HasColumnType("INTEGER");

                    b.Property<long>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Str")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Hash");

                    b.ToTable("CoinHash");
                });

            modelBuilder.Entity("WanBot.Plugin.WanCoin.WanCoinUser", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CoinCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
