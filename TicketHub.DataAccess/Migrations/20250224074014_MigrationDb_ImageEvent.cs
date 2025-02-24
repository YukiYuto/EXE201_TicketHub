using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_ImageEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventImage",
                table: "Events",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0aa11643-3343-45e5-b18f-52262c7b9552", "AQAAAAIAAYagAAAAEHXd0pyRxPn6C2XEw6jfji1Njk2mrdvv9qMGoKZhYMp0LnfyyZ7ghIdh3zehsIpVkg==", "05824abe-98cc-45d2-a957-35ed07a1aca2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d14560e5-b6c1-43ef-a2b7-db88b8740d6a", "AQAAAAIAAYagAAAAENAYO0MzF9ioY37L0NSY4zITrdzDHDGj/oTZDd0VPHAl5dpNMSmAC9/dQAwirTziKw==", "7570be09-4de0-4324-86d2-924561cdebdb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c7d157b9-0f7c-484d-9606-288dd1f4715c", "AQAAAAIAAYagAAAAEGtgQSGXkpMJ8Bl5bXfpxZP6amkNU0Tw1iH4Ne0zAzWvzKM+WlJpdMObLyMbRcgPDw==", "9db9e56f-eec1-4426-a04c-98ea749ecea1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventImage",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fff396d8-b090-4afe-b11b-a8f4e5aec659", "AQAAAAIAAYagAAAAEGG9bn2u/G6+GLotTtcRfG7NDes5g5JfJJ1u12uBRXviY+ZNhc8GldSZTDqYNimo9A==", "ccc551a6-8743-4786-9593-9ca91b002f1a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b9051f06-79f2-4228-9c4e-4bafd802b7b1", "AQAAAAIAAYagAAAAEBe+9D8XXWxtEbLD3bmVR9WpyKGeUjuocp+FUxOn0GqFQzo6T6u1YdyIulSnZ5EKTQ==", "b3d19811-42e5-4ab0-8f53-fa8feca4f1aa" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "341e26e8-a871-446e-964b-be2e84878dae", "AQAAAAIAAYagAAAAEA6fBSx+ijBP/WuGnnvRQ5n+hq7EbwOkA1D0amnIssZfndgEXORnhqBX5tIhGd4kIQ==", "f5f92edc-1262-4a00-8b18-10af4f04de68" });
        }
    }
}
