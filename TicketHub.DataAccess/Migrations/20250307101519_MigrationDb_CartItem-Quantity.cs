using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDb_CartItemQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CartItems");
        }
    }
}
