using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddedAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0c7e6621-2999-445d-bc94-b4dea1150a1d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bed946be-0c7c-4b8f-9222-b4b1e5e3d371");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "80af0388-33f3-4c1b-96ab-566e6853af13", null, "Member", "MEMBER" },
                    { "8ccadfbc-4954-42b5-a549-61fa3372aa6b", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "80af0388-33f3-4c1b-96ab-566e6853af13");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ccadfbc-4954-42b5-a549-61fa3372aa6b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0c7e6621-2999-445d-bc94-b4dea1150a1d", null, "Admin", "ADMIN" },
                    { "bed946be-0c7c-4b8f-9222-b4b1e5e3d371", null, "Member", "MEMBER" }
                });
        }
    }
}
