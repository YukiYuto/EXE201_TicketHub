using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TicketSerialNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "TicketSerialNumbers",
                columns: table => new
                {
                    SerialNumberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSerialNumbers", x => x.SerialNumberId);
                    table.ForeignKey(
                        name: "FK_TicketSerialNumbers_TicketTemplates_TicketTemplateId",
                        column: x => x.TicketTemplateId,
                        principalTable: "TicketTemplates",
                        principalColumn: "TicketTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketSerialNumbers_TicketTemplateId",
                table: "TicketSerialNumbers",
                column: "TicketTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketSerialNumbers");

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
                    { "ManagerId", 0, "789 Manager St", "https://example.com/avatarManager.png", new DateTime(1985, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "7dc7088e-fabe-48d5-b006-0a46962f98fb", "Country", "manager@gmail.com", true, "Manager User", true, null, "MANAGER@GMAIL.COM", "MANAGER@GMAIL.COM", "AQAAAAIAAYagAAAAEKjOyXzP1PhhVPppDJaZPd4TxZ2cNSKto66diM3M0MZbtV5ibe7LtNCuQ3vDEJM56g==", "0981234567", true, "ea518020-6c0d-4839-800b-936dab277b3f", false, "manager@gmail.com" },
                    { "StaffId", 0, "123 Staff St", "https://example.com/avatarStaff.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "844ea8e9-d1fa-4da3-996d-2d3c5c689816", "Country", "staff1@gmail.com", true, "Staff_1 User", true, null, "STAFF1@GMAIL.COM", "STAFF1@GMAIL.COM", "AQAAAAIAAYagAAAAEKwmJQaRs6hbIc+/F/mEe5wPDS78V46RrusfRs06XR1zwVJkeXnUn+xq/DkpxI1a6Q==", "0123456789", true, "0eb2ed95-4ed8-4d06-b7d9-b91527a28c61", false, "staff1@gmail.com" },
                    { "StaffId2", 0, "456 Staff St", "https://example.com/avatarStaff2.png", new DateTime(1991, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "7150b577-fe90-44c6-94ed-c98e9c12a267", "Country", "staff2@gmail.com", true, "Staff_2 User", true, null, "STAFF2@GMAIL.COM", "STAFF2@GMAIL.COM", "AQAAAAIAAYagAAAAEAHjW39qyFKIuCBFnnq5vVfbMCG140t2t/Si7gbouSGZhrXBPBhSJBCt4D+o7eqP8Q==", "0987654321", true, "a0a79629-d527-4874-be29-5f11d02c499e", false, "staff2@gmail.com" },
                    { "TicketHub-Admin", 0, "123 Admin St", "https://example.com/avatar.png", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "c6bb10d1-f116-45a1-8f9e-0a3be36d3905", "Country", "admin@gmail.com", true, "Admin User", true, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEOPmuw3CK2oqfztCpsWzuREweKYSCwdJKtAQg5XroTa7Bksr8Ybw3WK4bu2EB0SEcQ==", "1234567890", true, "3efedd7e-fce9-4231-be2a-8880ef4df5e5", false, "admin@gmail.com" }
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
    }
}
