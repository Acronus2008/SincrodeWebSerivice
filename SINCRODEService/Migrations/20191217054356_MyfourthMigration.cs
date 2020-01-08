using Microsoft.EntityFrameworkCore.Migrations;

namespace SINCRODEService.Migrations
{
    public partial class MyfourthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UBICEN_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "UbiCentrabajo_EMP");

            migrationBuilder.RenameColumn(
                name: "Teletrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "TeletrabajoEmp");

            migrationBuilder.RenameColumn(
                name: "JornadaFlexible_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "JornadaflexibleEmp");

            migrationBuilder.RenameColumn(
                name: "DESCDEP_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "DescDpto_EMP");

            migrationBuilder.RenameColumn(
                name: "Coworking_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "CoworkingEmp");

            migrationBuilder.RenameColumn(
                name: "CODDEP_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "CodDept_EMP");

            migrationBuilder.AlterColumn<string>(
                name: "PNRSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 20,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "NombreSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 50,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 50,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 50,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescNegocio",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 50,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescCentrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 50,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 20,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 20,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodNegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 20,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "ApellidosSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 50,
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldType: "string",
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "UbiCentrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 20,
                nullable: true,
                comment: "Nomenclatura de la ubicación del Centro de trabajo",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Nomenclatura de la ubicación del Centro de trabajo");

            migrationBuilder.AlterColumn<int>(
                name: "TeletrabajoEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "");

            migrationBuilder.AlterColumn<int>(
                name: "JornadaflexibleEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "");

            migrationBuilder.AlterColumn<int>(
                name: "CoworkingEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "");

            migrationBuilder.AddColumn<string>(
                name: "AD",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                maxLength: 50,
                nullable: true,
                comment: "Columna sin nombre");

            migrationBuilder.AddColumn<int>(
                name: "CodContrato_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                defaultValue: 0,
                comment: "");

            migrationBuilder.AddColumn<int>(
                name: "CoJornadaEsp_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: true,
                defaultValue: 0,
                comment: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AD",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "CodContrato_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.DropColumn(
                name: "CoJornadaEsp_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS");

            migrationBuilder.RenameColumn(
                name: "UbiCentrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "UBICEN_EMP");

            migrationBuilder.RenameColumn(
                name: "TeletrabajoEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "Teletrabajo_EMP");

            migrationBuilder.RenameColumn(
                name: "JornadaflexibleEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "JornadaFlexible_EMP");

            migrationBuilder.RenameColumn(
                name: "DescDpto_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "DESCDEP_EMP");

            migrationBuilder.RenameColumn(
                name: "CoworkingEmp",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "Coworking_EMP");

            migrationBuilder.RenameColumn(
                name: "CodDept_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                newName: "CODDEP_EMP");

            migrationBuilder.AlterColumn<string>(
                name: "PNRSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "NombreSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescNegocio",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "DescCentrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodSubnegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodSociedad_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodNegocio_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "ApellidosSup_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "string",
                nullable: true,
                comment: "",
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "");

            migrationBuilder.AlterColumn<string>(
                name: "UBICEN_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Nomenclatura de la ubicación del Centro de trabajo",
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Nomenclatura de la ubicación del Centro de trabajo");

            migrationBuilder.AlterColumn<int>(
                name: "Teletrabajo_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: false,
                comment: "",
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "JornadaFlexible_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: false,
                comment: "",
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "Coworking_EMP",
                schema: "dbo",
                table: "TBL_EMPLEADOS",
                type: "int",
                nullable: false,
                comment: "",
                oldClrType: typeof(int));
        }
    }
}
