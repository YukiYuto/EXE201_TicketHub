using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_Paymetn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<long>(type: "bigint", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancelUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiredAt = table.Column<long>(type: "bigint", nullable: true),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentTransactionId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderNumber",
                        column: x => x.OrderNumber,
                        principalTable: "Orders",
                        principalColumn: "OrderNumber",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderNumber",
                table: "Payments",
                column: "OrderNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Orders_OrderNumber",
                table: "Orders");

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
        }
    }
}
