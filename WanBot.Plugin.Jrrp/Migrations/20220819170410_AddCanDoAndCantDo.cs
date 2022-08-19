using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WanBot.Plugin.Jrrp.Migrations
{
    public partial class AddCanDoAndCantDo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CanDo",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantDo",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanDo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CantDo",
                table: "Users");
        }
    }
}
