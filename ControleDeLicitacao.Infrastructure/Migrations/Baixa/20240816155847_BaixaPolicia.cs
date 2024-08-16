using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class BaixaPolicia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaixaPolicia",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmpresaID = table.Column<int>(type: "int", nullable: false),
                    OrgaoID = table.Column<int>(type: "int", nullable: false),
                    DataLicitacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Vigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorLicitado = table.Column<double>(type: "float", nullable: false),
                    ValorEmpenhado = table.Column<double>(type: "float", nullable: false),
                    ValorEntregue = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaixaPolicia", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EmpenhoPolicia",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    NumEmpenho = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumNota = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DataEmpenho = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Valor = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpenhoPolicia", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmpenhoPolicia_BaixaPolicia_BaixaID",
                        column: x => x.BaixaID,
                        principalTable: "BaixaPolicia",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpenhoPolicia_BaixaID",
                table: "EmpenhoPolicia",
                column: "BaixaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpenhoPolicia");

            migrationBuilder.DropTable(
                name: "BaixaPolicia");
        }
    }
}
