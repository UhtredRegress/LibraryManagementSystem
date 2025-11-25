using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverAddress",
                table: "Books",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverAddress",
                table: "Books");
        }
    }
}
