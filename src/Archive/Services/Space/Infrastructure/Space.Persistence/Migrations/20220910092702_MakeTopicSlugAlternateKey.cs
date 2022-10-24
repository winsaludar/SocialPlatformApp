using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Persistence.Migrations
{
    public partial class MakeTopicSlugAlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Topic_Slug",
                table: "Topic",
                column: "Slug");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Topic_Slug",
                table: "Topic");
        }
    }
}
