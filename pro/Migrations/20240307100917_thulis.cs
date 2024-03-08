using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pro.Migrations
{
    /// <inheritdoc />
    public partial class thulis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Acknowledgments_UserId",
                table: "Acknowledgments");

            migrationBuilder.CreateIndex(
                name: "IX_Acknowledgments_UserId",
                table: "Acknowledgments",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Acknowledgments_UserId",
                table: "Acknowledgments");

            migrationBuilder.CreateIndex(
                name: "IX_Acknowledgments_UserId",
                table: "Acknowledgments",
                column: "UserId");
        }
    }
}
