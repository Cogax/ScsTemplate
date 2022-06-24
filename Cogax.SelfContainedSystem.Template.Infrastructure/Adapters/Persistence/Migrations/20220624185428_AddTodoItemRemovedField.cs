using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Migrations
{
    public partial class AddTodoItemRemovedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "TodoItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_Removed",
                table: "TodoItems",
                column: "Removed",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_Removed",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Removed",
                table: "TodoItems");
        }
    }
}
