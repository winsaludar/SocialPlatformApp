using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Persistence.Migrations
{
    public partial class AddSlugColumnOnSpaceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Space",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Space");
        }
    }
}
