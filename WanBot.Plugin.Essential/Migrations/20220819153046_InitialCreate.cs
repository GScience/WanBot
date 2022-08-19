using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WanBot.Plugin.Essential.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Money = table.Column<string>(type: "TEXT", nullable: false),
                    EnergyMax = table.Column<int>(type: "INTEGER", nullable: false),
                    Energy = table.Column<int>(type: "INTEGER", nullable: false),
                    LastTimeCheckEnergy = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AttactAddition = table.Column<int>(type: "INTEGER", nullable: false),
                    DefenceAddition = table.Column<int>(type: "INTEGER", nullable: false),
                    SpAttactAddition = table.Column<int>(type: "INTEGER", nullable: false),
                    SpDefenceAddition = table.Column<int>(type: "INTEGER", nullable: false),
                    HpAddition = table.Column<int>(type: "INTEGER", nullable: false),
                    MagicAddition = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
