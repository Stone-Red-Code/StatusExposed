using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusExposed.Migrations
{
    public partial class AddIdtoHistoryData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusHistoryData",
                table: "StatusHistoryData");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StatusHistoryData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusHistoryData",
                table: "StatusHistoryData",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusHistoryData",
                table: "StatusHistoryData");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StatusHistoryData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusHistoryData",
                table: "StatusHistoryData",
                column: "LastUpdateTime");
        }
    }
}
