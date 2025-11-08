using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceSaga.Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountAndItemsJsonToOrderState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderItemJson",
                table: "OrderStateInstance",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "OrderStateInstance",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderItemJson",
                table: "OrderStateInstance");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "OrderStateInstance");
        }
    }
}
