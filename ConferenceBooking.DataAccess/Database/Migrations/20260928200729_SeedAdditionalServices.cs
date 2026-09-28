using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConferenceBooking.DataAccess.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdditionalServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AdditionalServices",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-4111-8111-111111111111"), "Проєктор" },
                    { new Guid("22222222-2222-4222-8222-222222222222"), "Wi-Fi" },
                    { new Guid("33333333-3333-4333-8333-333333333333"), "Звук" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AdditionalServices",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-4111-8111-111111111111"));

            migrationBuilder.DeleteData(
                table: "AdditionalServices",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-4222-8222-222222222222"));

            migrationBuilder.DeleteData(
                table: "AdditionalServices",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-4333-8333-333333333333"));
        }
    }
}
