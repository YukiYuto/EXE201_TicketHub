using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_Transaction : Migration
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

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    TransactionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_Transactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "PaymentTransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CustomerId",
                table: "Transactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OrderId",
                table: "Transactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

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
