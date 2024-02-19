using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookstore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class company_seeded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "City1", "Entity1", "123-456-7890", "12345", "State1", "Street1" },
                    { 2, "City2", "Entity2", "987-654-3210", "54321", "State2", "Street2" },
                    { 3, "City3", "Entity3", "111-222-3333", "67890", "State3", "Street3" },
                    { 4, "City4", "Entity4", "444-555-6666", "09876", "State4", "Street4" },
                    { 5, "City5", "Entity5", "777-888-9999", "54321", "State5", "Street5" },
                    { 6, "City6", "Entity6", "000-111-2222", "13579", "State6", "Street6" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
