using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiMailServer.Migrations
{
    /// <inheritdoc />
    public partial class updatedMessageMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "producer_email",
                table: "Messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "producer_email",
                table: "Messages");
        }
    }
}
