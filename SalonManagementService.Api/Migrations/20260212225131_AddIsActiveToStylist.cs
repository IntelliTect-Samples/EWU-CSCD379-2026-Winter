using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonManagementService.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToStylist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Stylists",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Stylists");
        }
    }
}
