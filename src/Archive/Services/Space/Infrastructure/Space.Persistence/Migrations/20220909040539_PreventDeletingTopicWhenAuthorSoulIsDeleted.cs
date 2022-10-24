using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Persistence.Migrations
{
    public partial class PreventDeletingTopicWhenAuthorSoulIsDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Soul_SoulId",
                table: "Topic");

            migrationBuilder.AlterColumn<Guid>(
                name: "SoulId",
                table: "Topic",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Soul_SoulId",
                table: "Topic",
                column: "SoulId",
                principalTable: "Soul",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Soul_SoulId",
                table: "Topic");

            migrationBuilder.AlterColumn<Guid>(
                name: "SoulId",
                table: "Topic",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Soul_SoulId",
                table: "Topic",
                column: "SoulId",
                principalTable: "Soul",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
