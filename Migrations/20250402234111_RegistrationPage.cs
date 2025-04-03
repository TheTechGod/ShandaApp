using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShandaApp.Migrations
{
    /// <inheritdoc />
    public partial class RegistrationPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AIMood",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIMood",
                table: "AspNetUsers");
        }
    }
}
