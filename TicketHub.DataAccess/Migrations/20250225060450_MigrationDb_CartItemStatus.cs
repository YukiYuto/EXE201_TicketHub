using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_CartItemStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ac1dce37-a5a5-40c6-9f26-efbf999ef0ac", "AQAAAAIAAYagAAAAENrrTeUq18VDskQHw6fJeIKnPSIbesJr11lXwmal3jGj6kYbLdU0WtJ7sEOLseNVRw==", "c4ca7dbd-dd09-4b68-a6a5-29731f3b12c4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1b8dd84a-835b-48b9-acb2-a92e6074b0d9", "AQAAAAIAAYagAAAAEHcNgygIxZj+dOnqI1i9uzb2enz/VMido8BwzFmEWgngG2GqBQ2C7mULRv5k7DUxdQ==", "07ad8086-375a-4e81-b308-717dd4792608" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "212cfbb0-d527-41fa-9d17-9dbfe13602f1", "AQAAAAIAAYagAAAAEKTqnMAzmvi20d00k3ucE1HxlPQZm+pYdWEqk2jJMYoUDEdy7z1N/0fUDkbEi2iNHg==", "cee85260-ab74-4137-b694-f26efe4df392" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CartItems");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9d35267d-ec0b-4ab8-a8e4-f5f5a8460124", "AQAAAAIAAYagAAAAEDe5bz2+tu2QozuCqKBDNg3qVgr0YYQO0AkCpGgs8br3QmHau5oHyPRXjjpCfdmCdQ==", "a196193e-bea5-4636-9d28-82869f804e6a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fbeeab7d-5275-43ee-950d-1a30617f9dd6", "AQAAAAIAAYagAAAAEA6Ln/u6L9mkmoFlDh/APxRBqkhLhF6A3MDZVFVjvOQ2gMgq8KCuvFlHXPLUIaM4wA==", "2b36a167-ad51-4328-a34f-84b7c44d643c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "df3a5761-8c67-4036-b20c-caf910bc26bf", "AQAAAAIAAYagAAAAEL0ADxM9Yb6V1ETtXwUKLkTD78jmkXoYRUV0/kZli+Y+80evI3/AHAkalwEJW7oBBA==", "e79f741a-c7eb-447e-beb3-9b595b11c899" });
        }
    }
}
