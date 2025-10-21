using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CMCS.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class SeedLecturers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lecturers",
                columns: new[] { "LecturerId", "Email", "FullName" },
                values: new object[,]
                {
                    { 1, "asmith@college.edu", "A. Smith" },
                    { 2, "bnaidoo@college.edu", "B. Naidoo" },
                    { 3, "cdlamini@college.edu", "C. Dlamini" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "LecturerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "LecturerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "LecturerId",
                keyValue: 3);
        }
    }
}
