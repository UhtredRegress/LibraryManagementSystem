using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.BorrowService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBorrowHistorySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "BorrowHistories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnedConfirmBy",
                table: "BorrowHistories",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "BorrowHistories");

            migrationBuilder.DropColumn(
                name: "ReturnedConfirmBy",
                table: "BorrowHistories");
        }
    }
}
