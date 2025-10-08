using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.BookService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBookPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookPrices",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    BookType = table.Column<int>(type: "integer", nullable: false),
                    PriceUnit = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookPrices", x => new { x.BookId, x.BookType });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookPrices");
        }
    }
}
