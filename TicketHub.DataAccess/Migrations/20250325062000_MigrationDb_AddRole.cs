using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_AddRole : Migration
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
                    { "8fa7c7bb-daa5-a660-bf02-82301a5eb32a", "ADMIN", "ADMIN", "ADMIN" },
                    { "a7782126-d76b-41c9-86d9-f41a026d107d", "MANAGER", "MANAGER", "MANAGER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "AvatarUrl", "BirthDate", "ConcurrencyStamp", "Country", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "ManagerId", 0, "789 Manager St", "https://example.com/avatarManager.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "76d43009-ae9f-4a79-98e0-225b0fc43b80", "Country", "manager@gmail.com", true, "Manager User", true, null, "MANAGER@GMAIL.COM", "MANAGER@GMAIL.COM", "AQAAAAIAAYagAAAAEEiunn1vvx20boXyLXpUTHIrVsKGkBl2C7faGE2G4xEyi+cwoWighsgvUt6i/rHMNg==", "0981234567", true, "225dde77-bc41-427b-a6c9-c82ac554980a", false, "manager@gmail.com" },
                    { "StaffId", 0, "123 Staff St", "https://example.com/avatarStaff.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "9c3a311c-3580-43a4-b8e2-63a2a5f21a84", "Country", "staff1@gmail.com", true, "Staff_1 User", true, null, "STAFF1@GMAIL.COM", "STAFF1@GMAIL.COM", "AQAAAAIAAYagAAAAECZ1TSR1q6UEJw17XriNFbmN9Gp97bYkYuEP004lr8kYmYhkRSG0XaWzwMPMNP1AxA==", "0123456789", true, "1182ec60-a4d1-4d8c-8811-b5263c335705", false, "staff1@gmail.com" },
                    { "StaffId2", 0, "456 Staff St", "https://example.com/avatarStaff2.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "c741bb0e-0743-48f5-ad6b-677b08c73d68", "Country", "staff2@gmail.com", true, "Staff_2 User", true, null, "STAFF2@GMAIL.COM", "STAFF2@GMAIL.COM", "AQAAAAIAAYagAAAAEEfDOq19dMWzH7iXi6rjJmkvutrOn9fuRqk0f3nQJtCgA0Q+feLPnROfi2rtppOAvQ==", "0987654321", true, "b356e3a0-125b-4b4d-b3cc-1514c775a8aa", false, "staff2@gmail.com" },
                    { "TicketHub-Admin", 0, "123 Admin St", "https://example.com/avatar.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "9b64b0ce-51cf-4504-b3f9-cb2bcf859f5d", "Country", "admin@gmail.com", true, "Admin User", true, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEKmcYfiSKoK3ibPoO5PoArUZ6kkAkUIS3zeH8nueapmE5Fg+hYX2xNGruJgClZFbxg==", "1234567890", true, "1af6f4dd-f151-4772-a870-5fd95e3a3c80", false, "admin@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "a7782126-d76b-41c9-86d9-f41a026d107d", "ManagerId" },
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
                keyValues: new object[] { "a7782126-d76b-41c9-86d9-f41a026d107d", "ManagerId" });

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
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7782126-d76b-41c9-86d9-f41a026d107d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ManagerId");

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
