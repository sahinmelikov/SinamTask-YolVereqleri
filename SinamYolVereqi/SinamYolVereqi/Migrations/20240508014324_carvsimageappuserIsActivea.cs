using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinamYolVereqi.Migrations
{
    public partial class carvsimageappuserIsActivea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SahibkariName",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SahibkariName",
                table: "Cars");
        }
    }
}
