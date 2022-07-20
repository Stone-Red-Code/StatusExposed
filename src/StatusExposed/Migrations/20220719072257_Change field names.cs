using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusExposed.Migrations
{
    public partial class Changefieldnames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriber_Services_StatusInformationServicePageDomain",
                table: "Subscriber");

            migrationBuilder.DropTable(
                name: "StatusHistoryData");

            migrationBuilder.RenameColumn(
                name: "StatusInformationServicePageDomain",
                table: "Subscriber",
                newName: "ServiceInformationServicePageDomain");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriber_StatusInformationServicePageDomain",
                table: "Subscriber",
                newName: "IX_Subscriber_ServiceInformationServicePageDomain");

            migrationBuilder.CreateTable(
                name: "StatusData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResponseTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    ServiceInformationServicePageDomain = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusData_Services_ServiceInformationServicePageDomain",
                        column: x => x.ServiceInformationServicePageDomain,
                        principalTable: "Services",
                        principalColumn: "ServicePageDomain");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusData_ServiceInformationServicePageDomain",
                table: "StatusData",
                column: "ServiceInformationServicePageDomain");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriber_Services_ServiceInformationServicePageDomain",
                table: "Subscriber",
                column: "ServiceInformationServicePageDomain",
                principalTable: "Services",
                principalColumn: "ServicePageDomain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriber_Services_ServiceInformationServicePageDomain",
                table: "Subscriber");

            migrationBuilder.DropTable(
                name: "StatusData");

            migrationBuilder.RenameColumn(
                name: "ServiceInformationServicePageDomain",
                table: "Subscriber",
                newName: "StatusInformationServicePageDomain");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriber_ServiceInformationServicePageDomain",
                table: "Subscriber",
                newName: "IX_Subscriber_StatusInformationServicePageDomain");

            migrationBuilder.CreateTable(
                name: "StatusHistoryData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastUpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ping = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusInformationServicePageDomain = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusHistoryData", x => x.Id);
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

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriber_Services_StatusInformationServicePageDomain",
                table: "Subscriber",
                column: "StatusInformationServicePageDomain",
                principalTable: "Services",
                principalColumn: "ServicePageDomain");
        }
    }
}
