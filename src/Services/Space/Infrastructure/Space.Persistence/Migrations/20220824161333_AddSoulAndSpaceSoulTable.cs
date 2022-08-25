using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Persistence.Migrations
{
    public partial class AddSoulAndSpaceSoulTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Soul",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soul", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpaceSoul",
                columns: table => new
                {
                    SpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoulId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "IX_Soul_Email",
                table: "Soul",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soul_Name",
                table: "Soul",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpaceSoul_SpaceId",
                table: "SpaceSoul",
                column: "SpaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpaceSoul");

            migrationBuilder.DropTable(
                name: "Soul");
        }
    }
}
