using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinamYolVereqi.Migrations
{
    public partial class kjsaj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_YolSenedis_YolSenediId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_YolSenediId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "YolSenediId",
                table: "Cars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YolSenediId",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_YolSenediId",
                table: "Cars",
                column: "YolSenediId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_YolSenedis_YolSenediId",
                table: "Cars",
                column: "YolSenediId",
                principalTable: "YolSenedis",
                principalColumn: "Id");
        }
    }
}
