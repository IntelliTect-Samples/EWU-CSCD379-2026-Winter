using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonManagementService.Api.Migrations
{
    /// <inheritdoc />
    public partial class StylistImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Stylists",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Stylists");
        }
    }
}
