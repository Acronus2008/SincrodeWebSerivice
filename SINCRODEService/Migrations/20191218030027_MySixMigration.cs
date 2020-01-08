using Microsoft.EntityFrameworkCore.Migrations;

namespace SINCRODEService.Migrations
{
    public partial class MySixMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Numero_EMP",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO");

            migrationBuilder.DropColumn(
                name: "NumeroMensaje_MAR",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO");

            migrationBuilder.DropColumn(
                name: "TextoMensaje_MAR",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Numero_EMP",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                comment: "Numero de Empleado obtenido de la consulta a Dassnet");

            migrationBuilder.AddColumn<int>(
                name: "NumeroMensaje_MAR",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Número del mensaje");

            migrationBuilder.AddColumn<string>(
                name: "TextoMensaje_MAR",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO",
                type: "varchar(60)",
                unicode: false,
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                comment: "Texto del Mensaje");
        }
    }
}
