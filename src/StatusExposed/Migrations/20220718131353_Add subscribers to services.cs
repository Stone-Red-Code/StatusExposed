using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusExposed.Migrations
{
    public partial class Addsubscriberstoservices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscriber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    StatusInformationServicePageDomain = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriber_Services_StatusInformationServicePageDomain",
                        column: x => x.StatusInformationServicePageDomain,
                        principalTable: "Services",
                        principalColumn: "ServicePageDomain");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriber_StatusInformationServicePageDomain",
                table: "Subscriber",
                column: "StatusInformationServicePageDomain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscriber");
        }
    }
}
