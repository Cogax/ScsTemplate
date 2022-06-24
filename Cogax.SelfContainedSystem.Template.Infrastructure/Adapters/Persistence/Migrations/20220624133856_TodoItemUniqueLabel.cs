using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Migrations
{
    public partial class TodoItemUniqueLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_Label",
                table: "TodoItems",
                column: "Label",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_Label",
                table: "TodoItems");
        }
    }
}
