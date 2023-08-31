using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agriculture.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReviewInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsReview",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReview",
                table: "Orders");
        }
    }
}
