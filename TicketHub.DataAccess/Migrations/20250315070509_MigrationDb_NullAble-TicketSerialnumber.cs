using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_NullAbleTicketSerialnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketSerialNumbers_TicketTemplates_TicketTemplateId",
                table: "TicketSerialNumbers");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketTemplateId",
                table: "TicketSerialNumbers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSerialNumbers_TicketTemplates_TicketTemplateId",
                table: "TicketSerialNumbers",
                column: "TicketTemplateId",
                principalTable: "TicketTemplates",
                principalColumn: "TicketTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketSerialNumbers_TicketTemplates_TicketTemplateId",
                table: "TicketSerialNumbers");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketTemplateId",
                table: "TicketSerialNumbers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSerialNumbers_TicketTemplates_TicketTemplateId",
                table: "TicketSerialNumbers",
                column: "TicketTemplateId",
                principalTable: "TicketTemplates",
                principalColumn: "TicketTemplateId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
