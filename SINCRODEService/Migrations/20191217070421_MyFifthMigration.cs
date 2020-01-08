using Microsoft.EntityFrameworkCore.Migrations;

namespace SINCRODEService.Migrations
{
    public partial class MyFifthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoworkingEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornadaflexibleEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "TeletrabajoEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.AddColumn<int>(
                name: "Coworking_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                nullable: true,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coworking_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.AddColumn<int>(
                name: "CoworkingEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JornadaflexibleEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeletrabajoEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }
    }
}
