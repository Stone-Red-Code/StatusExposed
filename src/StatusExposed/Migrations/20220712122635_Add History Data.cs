using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusExposed.Migrations
{
    public partial class AddHistoryData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdateTime",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Ping",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Services");

            migrationBuilder.CreateTable(
                name: "StatusHistoryData",
                columns: table => new
                {
                    LastUpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Ping = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    StatusInformationServicePageDomain = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusHistoryData", x => x.LastUpdateTime);
                    table.ForeignKey(
                        name: "FK_StatusHistoryData_Services_StatusInformationServicePageDomain",
                        column: x => x.StatusInformationServicePageDomain,
                        principalTable: "Services",
                        principalColumn: "ServicePageDomain");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusHistoryData_StatusInformationServicePageDomain",
                table: "StatusHistoryData",
                column: "StatusInformationServicePageDomain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusHistoryData");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateTime",
                table: "Services",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Ping",
                table: "Services",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
