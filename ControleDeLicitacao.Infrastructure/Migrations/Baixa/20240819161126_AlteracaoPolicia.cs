using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class AlteracaoPolicia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpenhoPolicia_BaixaPolicia_BaixaID",
                table: "EmpenhoPolicia");

            migrationBuilder.DropTable(
                name: "BaixaPolicia");

            migrationBuilder.DropIndex(
                name: "IX_EmpenhoPolicia_BaixaID",
                table: "EmpenhoPolicia");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaixaPolicia",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    DataAta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataLicitacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmpresaID = table.Column<int>(type: "int", nullable: false),
                    OrgaoID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ValorEmpenhado = table.Column<double>(type: "float", nullable: false),
                    ValorEntregue = table.Column<double>(type: "float", nullable: false),
                    ValorLicitado = table.Column<double>(type: "float", nullable: false),
                    Vigencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaixaPolicia", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpenhoPolicia_BaixaID",
                table: "EmpenhoPolicia",
                column: "BaixaID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpenhoPolicia_BaixaPolicia_BaixaID",
                table: "EmpenhoPolicia",
                column: "BaixaID",
                principalTable: "BaixaPolicia",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
