using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class InitialCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaixaLicitacao",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmpresaID = table.Column<int>(type: "int", nullable: false),
                    OrgaoID = table.Column<int>(type: "int", nullable: false),
                    DataLicitacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Vigencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaixaLicitacao", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Empenho",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataEmpenho = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Saldo = table.Column<double>(type: "float", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empenho", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Empenho_BaixaLicitacao_BaixaID",
                        column: x => x.BaixaID,
                        principalTable: "BaixaLicitacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemDeBaixa",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    QtdeEmpenhada = table.Column<double>(type: "float", nullable: false),
                    QtdeLicitada = table.Column<double>(type: "float", nullable: false),
                    QtdeAEmpenhar = table.Column<double>(type: "float", nullable: false),
                    ValorEmpenhado = table.Column<double>(type: "float", nullable: false),
                    ValorLicitado = table.Column<double>(type: "float", nullable: false),
                    Saldo = table.Column<double>(type: "float", nullable: false),
                    ValorUnitario = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDeBaixa", x => new { x.BaixaID, x.ID });
                    table.ForeignKey(
                        name: "FK_ItemDeBaixa_BaixaLicitacao_BaixaID",
                        column: x => x.BaixaID,
                        principalTable: "BaixaLicitacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empenho_BaixaID",
                table: "Empenho",
                column: "BaixaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empenho");

            migrationBuilder.DropTable(
                name: "ItemDeBaixa");

            migrationBuilder.DropTable(
                name: "BaixaLicitacao");
        }
    }
}
