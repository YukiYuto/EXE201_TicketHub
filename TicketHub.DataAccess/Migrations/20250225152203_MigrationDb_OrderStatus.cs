using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_OrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "34ec66f2-fd28-4461-854a-9f75d706c227", "AQAAAAIAAYagAAAAECTnPsjvhkuklXlSZZP6OgN2eekCJZNpE38mG5/ej6G0vw0QlDyi752yyccugB8fmg==", "b44bf258-d483-4bf8-a214-a7d5260381e0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6e0931e0-40b7-41da-b8ac-d54ac98b5105", "AQAAAAIAAYagAAAAEMXEr9X1jhnjpzsEZ5JcSmHyUeWmElEVATlEiW5tg3VGTQO2q1NGB/+CcFxsk3pNTA==", "94930ab4-4816-44ab-8907-a5c53ed1ee71" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a4943008-7c75-424c-9549-96d96f00e79d", "AQAAAAIAAYagAAAAEGtbpaLdzvHSkBJ8yQxDueo0S4G+VIGrjI66mPiJ1HaVjQkjRzAGab2F8CzArj0J0w==", "6b2a716b-498a-4fb6-88ec-373ba2071dfe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "283ccf86-4349-4c70-a834-a9e9590f0d3c", "AQAAAAIAAYagAAAAEHErcnM5uNq5/0+COLsZm9q+wQ18Id9RntHljJycAUpHsLTyN0ofvCMro7gl+2DJxg==", "291f9418-1a36-44a0-adfe-97436e9f9207" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "812d0b10-6d77-4f3c-bec2-c6be3690e992", "AQAAAAIAAYagAAAAEGKAx34ezG42RVVpwtz5q8zyPw8ndMTv/H1LStBgzkpAV5bFEKNuAZzEXFYfQYbuLA==", "ae706068-30c8-48ff-85d0-7633577e6b20" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "901f11de-3bb5-4ac1-bce0-ee328852c5e8", "AQAAAAIAAYagAAAAEHvr2NloIT9NmvIjFDBW3LD7uaGG5M4fuY3yZ3DOHsn4DuU7ClPhrJ8wPkm2II74GA==", "f57e6db4-89ef-4b55-94b9-bb8a58f266e5" });
        }
    }
}
