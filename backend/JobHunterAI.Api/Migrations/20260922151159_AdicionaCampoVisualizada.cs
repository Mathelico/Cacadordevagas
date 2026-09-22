using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunterAI.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCampoVisualizada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visualizada",
                table: "Vagas",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visualizada",
                table: "Vagas");
        }
    }
}
