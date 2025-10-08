using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.BookService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeFieldForBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerUnit",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "availabily",
                table: "Books",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Books",
                newName: "FileAddress");

            migrationBuilder.AlterColumn<int>(
                name: "Stock",
                table: "Books",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Books",
                newName: "availabily");

            migrationBuilder.RenameColumn(
                name: "FileAddress",
                table: "Books",
                newName: "FileName");

            migrationBuilder.AlterColumn<int>(
                name: "Stock",
                table: "Books",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerUnit",
                table: "Books",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
