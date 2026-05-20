using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSlikaToJelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Jela",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Jela",
                keyColumn: "Id",
                keyValue: 1,
                column: "SlikaUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Jela",
                keyColumn: "Id",
                keyValue: 2,
                column: "SlikaUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Jela",
                keyColumn: "Id",
                keyValue: 3,
                column: "SlikaUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Jela",
                keyColumn: "Id",
                keyValue: 4,
                column: "SlikaUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Jela",
                keyColumn: "Id",
                keyValue: 5,
                column: "SlikaUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Jela");
        }
    }
}
