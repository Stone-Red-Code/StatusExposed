using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusExposed.Migrations
{
    public partial class Addusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    SessionToken = table.Column<string>(type: "TEXT", nullable: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_UserId",
                table: "Permission",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Users");

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
    }
}
