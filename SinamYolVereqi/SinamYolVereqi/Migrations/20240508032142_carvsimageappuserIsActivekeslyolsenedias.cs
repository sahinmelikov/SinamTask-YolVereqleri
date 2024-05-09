using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinamYolVereqi.Migrations
{
    public partial class carvsimageappuserIsActivekeslyolsenedias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YeniSahibkarEmaili",
                table: "YolSenedis",
                newName: "TehvilAlaninEmaili");

            migrationBuilder.RenameColumn(
                name: "YeniSahibkarAdi",
                table: "YolSenedis",
                newName: "TehvilAlaninAdi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TehvilAlaninEmaili",
                table: "YolSenedis",
                newName: "YeniSahibkarEmaili");

            migrationBuilder.RenameColumn(
                name: "TehvilAlaninAdi",
                table: "YolSenedis",
                newName: "YeniSahibkarAdi");
        }
    }
}
