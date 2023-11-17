using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mongo.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class READDTABLE2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "CartDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "CartDetails");
        }
    }
}
