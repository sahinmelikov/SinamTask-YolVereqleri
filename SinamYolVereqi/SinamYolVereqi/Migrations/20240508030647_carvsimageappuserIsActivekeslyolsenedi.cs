using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinamYolVereqi.Migrations
{
    public partial class carvsimageappuserIsActivekeslyolsenedi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YolSenedis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nomre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeriyaNomresi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YeniSahibkarAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YeniSahibkarEmaili = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SahibkarinAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerilmeMuddeti = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YolSenedis", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YolSenedis");
        }
    }
}
