using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusExposed.Migrations
{
    public partial class AddAPIkeyfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ApiKey",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "ApiKey");
        }
    }
}
