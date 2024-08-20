using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingCartService.API.Migrations
{
    /// <inheritdoc />
    public partial class addingCartTotal2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartTotal",
                table: "CartHeaders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CartTotal",
                table: "CartHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
