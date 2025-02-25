using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_CartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.AddColumn<Guid>(
                name: "CartItemId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TicketId1",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                column: "CartItemId");

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

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_TicketId1",
                table: "CartItems",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Tickets_TicketId1",
                table: "CartItems",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Tickets_TicketId1",
                table: "CartItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_TicketId1",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "CartItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                columns: new[] { "CartId", "TicketId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
