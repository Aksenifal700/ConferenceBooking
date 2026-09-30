using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConferenceBooking.DataAccess.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "HourlyRate", "IsArchived", "RoomName" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"), 50, 2000m, false, "Зал A" },
                    { new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"), 100, 3500m, false, "Зал B" },
                    { new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc"), 30, 1500m, false, "Зал C" }
                });

            migrationBuilder.InsertData(
                table: "RoomServices",
                columns: new[] { "AdditionalServiceId", "RoomId", "Price" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-4111-8111-111111111111"), new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"), 500m },
                    { new Guid("22222222-2222-4222-8222-222222222222"), new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"), 300m },
                    { new Guid("33333333-3333-4333-8333-333333333333"), new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"), 700m },
                    { new Guid("11111111-1111-4111-8111-111111111111"), new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"), 500m },
                    { new Guid("22222222-2222-4222-8222-222222222222"), new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"), 300m },
                    { new Guid("33333333-3333-4333-8333-333333333333"), new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"), 700m },
                    { new Guid("11111111-1111-4111-8111-111111111111"), new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc"), 500m },
                    { new Guid("22222222-2222-4222-8222-222222222222"), new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc"), 300m },
                    { new Guid("33333333-3333-4333-8333-333333333333"), new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc"), 700m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("11111111-1111-4111-8111-111111111111"), new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("22222222-2222-4222-8222-222222222222"), new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("33333333-3333-4333-8333-333333333333"), new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("11111111-1111-4111-8111-111111111111"), new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("22222222-2222-4222-8222-222222222222"), new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("33333333-3333-4333-8333-333333333333"), new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("11111111-1111-4111-8111-111111111111"), new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("22222222-2222-4222-8222-222222222222"), new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "RoomServices",
                keyColumns: new[] { "AdditionalServiceId", "RoomId" },
                keyValues: new object[] { new Guid("33333333-3333-4333-8333-333333333333"), new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-4ccc-8ccc-cccccccccccc"));
        }
    }
}
