using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinamYolVereqi.Migrations
{
    public partial class carvsimageappuserIsActivek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SahibkariName",
                table: "Cars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SahibkariName",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
