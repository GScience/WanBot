using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WanBot.Plugin.WanCoin.Migrations
{
    public partial class AddTrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trade",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User = table.Column<long>(type: "INTEGER", nullable: false),
                    Group = table.Column<long>(type: "INTEGER", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<string>(type: "TEXT", nullable: false),
                    CoinCount = table.Column<long>(type: "INTEGER", nullable: false),
                    TotalCount = table.Column<long>(type: "INTEGER", nullable: false),
                    IsBuy = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trade", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trade");
        }
    }
}
