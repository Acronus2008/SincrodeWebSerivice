using Microsoft.EntityFrameworkCore.Migrations;

namespace SINCRODEService.Migrations
{
    public partial class MythirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApellidosSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(100)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "CodNegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(20)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "CodSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(20)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "CodSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(20)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "Coworking_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "DescCentrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(50)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "DescNegocio",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(50)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "DescSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(50)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "DescSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(50)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornadaFlexible_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornLaboralDomingo",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornLaboralJueves",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornLaboralLunes",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornLaboralMartes",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornLaboralMiercoles",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornLaboralSabado",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "JornLaboralViernes",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(50)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<string>(
                name: "PNRSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(20)",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<double>(
                name: "PorcenJornada_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "float",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "Teletrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "TipoContrato_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                comment: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApellidosSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "CodNegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "CodSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "CodSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "Coworking_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "DescCentrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "DescNegocio",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "DescSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "DescSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornadaFlexible_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornLaboralDomingo",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornLaboralJueves",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornLaboralLunes",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornLaboralMartes",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornLaboralMiercoles",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornLaboralSabado",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "JornLaboralViernes",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "NombreSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "PNRSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "PorcenJornada_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "Teletrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "TipoContrato_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");
        }
    }
}
