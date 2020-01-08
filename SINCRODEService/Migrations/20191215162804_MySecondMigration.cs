using Microsoft.EntityFrameworkCore.Migrations;

namespace SINCRODEService.Migrations
{
    public partial class MySecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JornLaboral_Festivo",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                defaultValue: 0,
                comment: "Indica si la jornada laboral es festiva");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JornLaboral_Festivo",
                schema: "dbo",
                table: "TBL_EMPLEADOS");
        }
    }
}
