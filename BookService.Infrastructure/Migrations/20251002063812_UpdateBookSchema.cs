using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.BookService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Books",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Books");
        }
    }
}
