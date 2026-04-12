using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jela",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vrsta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cijena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dostupno = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jela", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zaposlenici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnickoIme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lozinka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uloga = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaposlenici", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Narudzbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumVrijeme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NacinPreuzimanja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UkupnaCijena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RazlogOdbijanja = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PredvijenoVrijemeDostave = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZaposlenikId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narudzbe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Narudzbe_Zaposlenici_ZaposlenikId",
                        column: x => x.ZaposlenikId,
                        principalTable: "Zaposlenici",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StavkeNarudzbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kolicina = table.Column<int>(type: "int", nullable: false),
                    CijenaStavke = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NarudzbaId = table.Column<int>(type: "int", nullable: false),
                    JeloId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkeNarudzbe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StavkeNarudzbe_Jela_JeloId",
                        column: x => x.JeloId,
                        principalTable: "Jela",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StavkeNarudzbe_Narudzbe_NarudzbaId",
                        column: x => x.NarudzbaId,
                        principalTable: "Narudzbe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Jela",
                columns: new[] { "Id", "Cijena", "Dostupno", "Naziv", "Opis", "Vrsta" },
                values: new object[,]
                {
                    { 1, 9.50m, true, "Margherita Pizza", "Klasična pizza s rajčicom i mozzarellom", "Hrana" },
                    { 2, 8.00m, true, "Hamburger", "Goveđi burger s povrćem", "Hrana" },
                    { 3, 2.50m, true, "Coca Cola", "0.33l", "Piće" },
                    { 4, 10.00m, true, "Pasta Carbonara", "Kremasta pasta s pancetom", "Hrana" },
                    { 5, 3.00m, true, "Sok od naranče", "Svježe cijeđeni", "Piće" }
                });

            migrationBuilder.InsertData(
                table: "Zaposlenici",
                columns: new[] { "Id", "Ime", "KorisnickoIme", "Lozinka", "Prezime", "Uloga" },
                values: new object[,]
                {
                    { 1, "Admin", "admin", "admin123", "Admin", "Administrator" },
                    { 2, "Pero", "pero", "pero123", "Perić", "Zaposlenik" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbe_ZaposlenikId",
                table: "Narudzbe",
                column: "ZaposlenikId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkeNarudzbe_JeloId",
                table: "StavkeNarudzbe",
                column: "JeloId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkeNarudzbe_NarudzbaId",
                table: "StavkeNarudzbe",
                column: "NarudzbaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StavkeNarudzbe");

            migrationBuilder.DropTable(
                name: "Jela");

            migrationBuilder.DropTable(
                name: "Narudzbe");

            migrationBuilder.DropTable(
                name: "Zaposlenici");
        }
    }
}
