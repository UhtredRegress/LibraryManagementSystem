using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.BookService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceFieldForBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerUnit",
                table: "Books",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerUnit",
                table: "Books");
        }
    }
}
