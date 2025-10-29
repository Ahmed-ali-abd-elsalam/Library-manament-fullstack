using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixuserTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_AspNetUsers_userId1",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_userId1",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "userId1",
                table: "UserTokens");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "UserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_userId",
                table: "UserTokens",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_AspNetUsers_userId",
                table: "UserTokens",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_AspNetUsers_userId",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_userId",
                table: "UserTokens");

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "UserTokens",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "userId1",
                table: "UserTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_userId1",
                table: "UserTokens",
                column: "userId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_AspNetUsers_userId1",
                table: "UserTokens",
                column: "userId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
