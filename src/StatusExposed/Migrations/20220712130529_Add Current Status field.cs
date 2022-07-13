using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusExposed.Migrations
{
    public partial class AddCurrentStatusfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStatusHistoryDataId",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Services_CurrentStatusHistoryDataId",
                table: "Services",
                column: "CurrentStatusHistoryDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_StatusHistoryData_CurrentStatusHistoryDataId",
                table: "Services",
                column: "CurrentStatusHistoryDataId",
                principalTable: "StatusHistoryData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_StatusHistoryData_CurrentStatusHistoryDataId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_CurrentStatusHistoryDataId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CurrentStatusHistoryDataId",
                table: "Services");
        }
    }
}
