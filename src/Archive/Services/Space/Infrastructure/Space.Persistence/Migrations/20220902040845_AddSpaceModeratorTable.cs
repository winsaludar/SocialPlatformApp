using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Persistence.Migrations
{
    public partial class AddSpaceModeratorTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpaceModerator",
                columns: table => new
                {
                    SpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoulId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceModerator", x => new { x.SoulId, x.SpaceId });
                    table.ForeignKey(
                        name: "FK_SpaceModerator_Soul_SoulId",
                        column: x => x.SoulId,
                        principalTable: "Soul",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpaceModerator_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Space",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceModerator_SpaceId",
                table: "SpaceModerator",
                column: "SpaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpaceModerator");
        }
    }
}
