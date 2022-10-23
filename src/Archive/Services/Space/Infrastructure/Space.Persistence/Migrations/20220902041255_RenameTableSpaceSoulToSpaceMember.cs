using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Persistence.Migrations
{
    public partial class RenameTableSpaceSoulToSpaceMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpaceSoul");

            migrationBuilder.CreateTable(
                name: "SpaceMember",
                columns: table => new
                {
                    SpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoulId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceMember", x => new { x.SoulId, x.SpaceId });
                    table.ForeignKey(
                        name: "FK_SpaceMember_Soul_SoulId",
                        column: x => x.SoulId,
                        principalTable: "Soul",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpaceMember_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Space",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceMember_SpaceId",
                table: "SpaceMember",
                column: "SpaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpaceMember");

            migrationBuilder.CreateTable(
                name: "SpaceSoul",
                columns: table => new
                {
                    SoulId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceSoul", x => new { x.SoulId, x.SpaceId });
                    table.ForeignKey(
                        name: "FK_SpaceSoul_Soul_SoulId",
                        column: x => x.SoulId,
                        principalTable: "Soul",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpaceSoul_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Space",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceSoul_SpaceId",
                table: "SpaceSoul",
                column: "SpaceId");
        }
    }
}
