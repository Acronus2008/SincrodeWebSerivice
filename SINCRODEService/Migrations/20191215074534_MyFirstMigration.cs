using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SINCRODEService.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "TBL_EMPLEADOS",
                schema: "dbo",
                columns: table => new
                {
                    ID_EMP = table.Column<int>(nullable: false, comment: "Id del empleado. Se utilizará como identificativo único del registro"),
                    Nombre_EMP = table.Column<string>(maxLength: 100, nullable: false, comment: "Contiene el nombre del empleado"),
                    Apellidos_EMP = table.Column<string>(maxLength: 100, nullable: false, comment: "Contiene el Apellido del empleado"),
                    DNI_EMP = table.Column<string>(maxLength: 20, nullable: false, comment: "Identificación fiscal del empleado"),
                    Numero_EMP = table.Column<string>(maxLength: 20, nullable: false, comment: "Número único de empleado"),
                    IDOracle_EMP = table.Column<string>(maxLength: 20, nullable: false, comment: "Identificador del empleado en la BD de Oracle"),
                    CODCEN_EMP = table.Column<string>(maxLength: 50, nullable: true, comment: "Código identificativo del Centro de Trabajo"),
                    UBICEN_EMP = table.Column<string>(maxLength: 50, nullable: true, comment: "Nomenclatura de la ubicación del Centro de trabajo"),
                    CODDEP_EMP = table.Column<string>(maxLength: 50, nullable: true, comment: "Código del Departamento del empleado"),
                    DESCDEP_EMP = table.Column<string>(maxLength: 50, nullable: true, comment: "Descripción o Nombre del Departamento del empleado")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_EMPLEADOS", x => x.ID_EMP);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PROCESOS",
                schema: "dbo",
                columns: table => new
                {
                    ID_PRO = table.Column<int>(nullable: false, comment: "Id del proceso realizado. Se utilizará como identificativo único del registro"),
                    FechaIni_PRO = table.Column<DateTime>(type: "datetime", nullable: false, comment: "Fecha y hora en que se inicia el proceso DD/MM/YYYY HH:MM"),
                    FechaFin_PRO = table.Column<DateTime>(type: "datetime", nullable: false, comment: "Fecha y hora en que se finaliza el proceso DD/MM/YYYY HH:MM"),
                    Registros_PRO = table.Column<int>(nullable: false, comment: "Numero de registros procesados"),
                    Empleados_PRO = table.Column<int>(nullable: false, comment: "Número de empleados procesados"),
                    Errores_PRO = table.Column<int>(nullable: false, comment: "Numero de errores, 0 en caso de éxito"),
                    AUTO_PRO = table.Column<bool>(nullable: false, defaultValueSql: "((1))", comment: "Si el proceso es automático o no, 1- SINCRONIZADORDE ó 2- VIEWERDE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PROCESOS", x => x.ID_PRO);
                });

            migrationBuilder.CreateTable(
                name: "TBL_MARCAJEPROCESADO",
                schema: "dbo",
                columns: table => new
                {
                    ID_MAR = table.Column<int>(nullable: false, comment: "Id del marcaje registrado. Se utilizará como identificativo único del registro"),
                    ID_EMP = table.Column<int>(nullable: false, comment: "Id del empleado. Se relaciona con el de la tabla TBL_EMPLEADOS"),
                    ID_PRO = table.Column<int>(nullable: false, comment: "Identificador del Proceso realizado"),
                    Numero_EMP = table.Column<string>(maxLength: 20, nullable: false, comment: "Numero de Empleado obtenido de la consulta a Dassnet"),
                    FechaMarcaje_MAR = table.Column<DateTime>(type: "datetime", nullable: false, comment: "Fecha de registro de acceso DD/MM/YYYY HH:MM"),
                    DNI_EMP = table.Column<string>(maxLength: 20, nullable: false, comment: "DNI del empleado según PersonasT"),
                    CodTarjeta_MAR = table.Column<string>(maxLength: 10, nullable: false, comment: "Codigo de la tarjeta utilizada"),
                    NumeroMensaje_MAR = table.Column<int>(nullable: false, comment: "Número del mensaje"),
                    TextoMensaje_MAR = table.Column<string>(unicode: false, maxLength: 60, nullable: false, comment: "Texto del Mensaje"),
                    IdLector_MAR = table.Column<string>(unicode: false, maxLength: 15, nullable: false, comment: "Codigo del lector"),
                    NombreLector_MAR = table.Column<string>(unicode: false, maxLength: 40, nullable: false, comment: "Nombre del lector")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_MARCAJEPROCESADO", x => x.ID_MAR);
                    table.ForeignKey(
                        name: "FK_TBL_MARCAJEPROCESADO_TBL_EMPLEADOS",
                        column: x => x.ID_EMP,
                        principalSchema: "dbo",
                        principalTable: "TBL_EMPLEADOS",
                        principalColumn: "ID_EMP",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_MARCAJEPROCESADO_TBL_PROCESOS",
                        column: x => x.ID_PRO,
                        principalSchema: "dbo",
                        principalTable: "TBL_PROCESOS",
                        principalColumn: "ID_PRO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PROCESOSLOG",
                schema: "dbo",
                columns: table => new
                {
                    ID_PROLOG = table.Column<int>(nullable: false, comment: "Id del log registrado tras la ejecución de un proceso. Se utilizará como identificativo único del registro"),
                    ID_PRO = table.Column<int>(nullable: false, comment: "Identificador del Proceso realizado"),
                    ID_EMP = table.Column<int>(nullable: false, comment: "Id del empleado según TBL_EMPLEADO"),
                    FechaIni_PRO = table.Column<DateTime>(type: "datetime", nullable: false, comment: "Fecha y hora en que se inicia el proceso"),
                    DESC_PROLOG = table.Column<string>(type: "text", nullable: true, comment: "Descripción del Error en el empleado"),
                    EXC_PROLOG = table.Column<string>(type: "text", nullable: true, comment: "Descripción de la Excepción del error en la operación realizada")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PROCESOSLOG", x => x.ID_PROLOG);
                    table.ForeignKey(
                        name: "FK_TBL_PROCESOSLOG_TBL_EMPLEADOS",
                        column: x => x.ID_EMP,
                        principalSchema: "dbo",
                        principalTable: "TBL_EMPLEADOS",
                        principalColumn: "ID_EMP",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_PROCESOSLOG_TBL_PROCESOS",
                        column: x => x.ID_PRO,
                        principalSchema: "dbo",
                        principalTable: "TBL_PROCESOS",
                        principalColumn: "ID_PRO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_MARCAJEPROCESADO_ID_EMP",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO",
                column: "ID_EMP");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_MARCAJEPROCESADO_ID_PRO",
                schema: "dbo",
                table: "TBL_MARCAJEPROCESADO",
                column: "ID_PRO");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PROCESOSLOG_ID_EMP",
                schema: "dbo",
                table: "TBL_PROCESOSLOG",
                column: "ID_EMP");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PROCESOSLOG_ID_PRO",
                schema: "dbo",
                table: "TBL_PROCESOSLOG",
                column: "ID_PRO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_MARCAJEPROCESADO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TBL_PROCESOSLOG",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TBL_EMPLEADOS",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TBL_PROCESOS",
                schema: "dbo");
        }
    }
}
