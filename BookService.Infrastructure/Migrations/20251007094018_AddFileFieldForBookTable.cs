using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.BookService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileFieldForBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Books",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Books");
        }
    }
}
