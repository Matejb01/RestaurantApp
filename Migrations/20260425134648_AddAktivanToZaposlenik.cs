using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAktivanToZaposlenik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Aktivan",
                table: "Zaposlenici",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Zaposlenici",
                keyColumn: "Id",
                keyValue: 1,
                column: "Aktivan",
                value: true);

            migrationBuilder.UpdateData(
                table: "Zaposlenici",
                keyColumn: "Id",
                keyValue: 2,
                column: "Aktivan",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aktivan",
                table: "Zaposlenici");
        }
    }
}
