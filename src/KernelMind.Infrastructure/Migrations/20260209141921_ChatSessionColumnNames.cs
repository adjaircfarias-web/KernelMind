using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KernelMind.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChatSessionColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                schema: "kernelmind",
                table: "pizzas",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "kernelmind",
                table: "pizzas",
                newName: "id");
        }
    }
}
