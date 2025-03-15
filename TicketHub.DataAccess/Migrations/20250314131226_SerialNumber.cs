using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SerialNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TicketDescription",
                table: "Tickets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "SerialNumberId",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SerialNumberId",
                table: "Tickets",
                column: "SerialNumberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketSerialNumbers_SerialNumberId",
                table: "Tickets",
                column: "SerialNumberId",
                principalTable: "TicketSerialNumbers",
                principalColumn: "SerialNumberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketSerialNumbers_SerialNumberId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_SerialNumberId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SerialNumberId",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "TicketDescription",
                table: "Tickets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
