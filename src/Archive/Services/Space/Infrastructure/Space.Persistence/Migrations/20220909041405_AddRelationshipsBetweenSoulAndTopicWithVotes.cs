using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Persistence.Migrations
{
    public partial class AddRelationshipsBetweenSoulAndTopicWithVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoulTopicVote",
                columns: table => new
                {
                    SoulId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Upvote = table.Column<int>(type: "int", nullable: false),
                    Downvote = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoulTopicVote", x => new { x.SoulId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_SoulTopicVote_Soul_SoulId",
                        column: x => x.SoulId,
                        principalTable: "Soul",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SoulTopicVote_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoulTopicVote_TopicId",
                table: "SoulTopicVote",
                column: "TopicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoulTopicVote");
        }
    }
}
