using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_Payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderNumber",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9495fe91-9e01-418a-a967-64f381603b40", "AQAAAAIAAYagAAAAEJH7CdrMtK+SrPoGdhCMgB9Z6f40epc8me8LPx/Ev74CZCr8wDSrsFeUob34NTIx6g==", "6e3d0fd3-327e-4e05-8a04-afa00e2ac5f0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "30cc1d23-27d6-4688-82df-ea7472ac6ae7", "AQAAAAIAAYagAAAAEJZA2Cz4c+giQYe4uTooRo2aiQ2pDDdSna64dYLvxdeOARgug+lXy1p2gQdoETdzXQ==", "7bb50a9d-c149-4111-abfc-2ea64ab44bc9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "347df0e2-2481-47f6-b4a1-32e7abebd810", "AQAAAAIAAYagAAAAEIQceoMM/6m0fJR3avDgpz9g6XiNqoJtJGvhHx0qn0VLEoJhbx3/F4RCcQqacldHxA==", "66b3aaf2-d87d-4560-af2a-ff66f161f201" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "322c7161-4fcd-470f-a3f4-377bb3573861", "AQAAAAIAAYagAAAAECYfTSpLQI7B3Gce/aWU0UeVEM6OKKwpKEqMivTAeDPBGYNMwxItZcsp3mInAH3p9A==", "23c53d27-072a-4375-abd1-5a050076fd6a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "StaffId2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5a18bf84-aec9-43c1-9668-99e6ac4dbf2b", "AQAAAAIAAYagAAAAEF84xFJIq+r9NnFYk7O3t45AhWreJGaB4krmbGcCtZhfvFLZdXXlM8QqcP3IQk010A==", "85054fb2-b37d-4f63-8be3-73b179b7b88d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "TicketHub-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6bdd37b1-9b3c-4bce-b2c4-89e694f6e4a9", "AQAAAAIAAYagAAAAECTEL00f4SFPe/TlBwjscPcDLxpNTF+cQlrUNoXFxd77JwE56V9xVZTQUdBVAlFyEA==", "5dbd9c1f-1456-4d51-b03a-645d6ab03b29" });
        }
    }
}
