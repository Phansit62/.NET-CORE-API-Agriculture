using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agriculture.Migrations
{
    /// <inheritdoc />
    public partial class FixCouponUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CouponUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CouponUsers");
        }
    }
}
