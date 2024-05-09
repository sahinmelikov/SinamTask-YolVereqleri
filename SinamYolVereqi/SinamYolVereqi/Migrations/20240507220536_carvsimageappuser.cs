using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinamYolVereqi.Migrations
{
    public partial class carvsimageappuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Cars",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserName",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_AppUserId",
                table: "Cars",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_AspNetUsers_AppUserId",
                table: "Cars",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_AspNetUsers_AppUserId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_AppUserId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "AppUserName",
                table: "Cars");
        }
    }
}
