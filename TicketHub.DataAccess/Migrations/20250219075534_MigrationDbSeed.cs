using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDbSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8fa7c7bb-b4dd-480d-a660-e07a90855d5s", "STAFF", "STAFF", "STAFF" },
                    { "8fa7c7bb-daa5-a660-bf02-82301a5eb32a", "ADMIN", "ADMIN", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "AvatarUrl", "BirthDate", "CCCD", "ConcurrencyStamp", "Country", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OrganizationName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TaxId", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { "StaffId", 0, "123 Staff St", "https://example.com/avatarStaff.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "123456789126", "322c7161-4fcd-470f-a3f4-377bb3573861", "Country", "staff1@gmail.com", true, "Staff_1 User", true, null, "STAFF1@GMAIL.COM", "STAFF1@GMAIL.COM", null, "AQAAAAIAAYagAAAAECYfTSpLQI7B3Gce/aWU0UeVEM6OKKwpKEqMivTAeDPBGYNMwxItZcsp3mInAH3p9A==", "0123456789", true, "23c53d27-072a-4375-abd1-5a050076fd6a", null, false, "staff1@gmail.com", null },
                    { "StaffId2", 0, "456 Staff St", "https://example.com/avatarStaff2.png", new DateTime(1991, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "123456789124", "5a18bf84-aec9-43c1-9668-99e6ac4dbf2b", "Country", "staff2@gmail.com", true, "Staff_2 User", true, null, "STAFF2@GMAIL.COM", "STAFF2@GMAIL.COM", null, "AQAAAAIAAYagAAAAEF84xFJIq+r9NnFYk7O3t45AhWreJGaB4krmbGcCtZhfvFLZdXXlM8QqcP3IQk010A==", "0987654321", true, "85054fb2-b37d-4f63-8be3-73b179b7b88d", null, false, "staff2@gmail.com", null },
                    { "TicketHub-Admin", 0, "123 Admin St", "https://example.com/avatar.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "123456789123", "6bdd37b1-9b3c-4bce-b2c4-89e694f6e4a9", "Country", "admin@gmail.com", true, "Admin User", true, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", null, "AQAAAAIAAYagAAAAECTEL00f4SFPe/TlBwjscPcDLxpNTF+cQlrUNoXFxd77JwE56V9xVZTQUdBVAlFyEA==", "1234567890", true, "5dbd9c1f-1456-4d51-b03a-645d6ab03b29", null, false, "admin@gmail.com", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "8fa7c7bb-b4dd-480d-a660-e07a90855d5s", "StaffId" },
                    { "8fa7c7bb-b4dd-480d-a660-e07a90855d5s", "StaffId2" },
                    { "8fa7c7bb-daa5-a660-bf02-82301a5eb32a", "TicketHub-Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8fa7c7bb-b4dd-480d-a660-e07a90855d5s", "StaffId" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8fa7c7bb-b4dd-480d-a660-e07a90855d5s", "StaffId2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8fa7c7bb-daa5-a660-bf02-82301a5eb32a", "TicketHub-Admin" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8fa7c7bb-b4dd-480d-a660-e07a90855d5s");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8fa7c7bb-daa5-a660-bf02-82301a5eb32a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin");
        }
    }
}
