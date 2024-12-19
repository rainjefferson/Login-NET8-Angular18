using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenConfirmeEmailToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenConfirmeEmail",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenConfirmeEmail",
                table: "Users");
        }
    }
}
