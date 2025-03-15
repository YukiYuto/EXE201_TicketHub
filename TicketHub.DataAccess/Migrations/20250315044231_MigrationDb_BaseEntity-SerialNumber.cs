using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_BaseEntitySerialNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TicketSerialNumbers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "TicketSerialNumbers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TicketSerialNumbers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TicketSerialNumbers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "TicketSerialNumbers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TicketSerialNumbers");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "TicketSerialNumbers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TicketSerialNumbers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TicketSerialNumbers");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "TicketSerialNumbers");
        }
    }
}
