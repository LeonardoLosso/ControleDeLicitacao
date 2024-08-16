using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class ReinicioModuloDocumentos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaixaLicitacao",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Unidade = table.Column<int>(type: "int", nullable: false),
                    EmpresaID = table.Column<int>(type: "int", nullable: false),
                    OrgaoID = table.Column<int>(type: "int", nullable: false),
                    DataLicitacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Vigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalReajustes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaixaLicitacao", x => x.ID);
                });

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
                name: "Empenho",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumEmpenho = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Unidade = table.Column<int>(type: "int", nullable: false),
                    OrgaoID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataEmpenho = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Saldo = table.Column<double>(type: "float", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empenho", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Nota",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumNota = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    NumEmpenho = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmpenhoID = table.Column<int>(type: "int", nullable: false),
                    Unidade = table.Column<int>(type: "int", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataEntrega = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nota", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reajuste",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtaID = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reajuste", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemDeBaixa",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<double>(type: "float", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    QtdeEmpenhada = table.Column<double>(type: "float", nullable: false),
                    QtdeLicitada = table.Column<double>(type: "float", nullable: false),
                    QtdeAEmpenhar = table.Column<double>(type: "float", nullable: false),
                    ValorEmpenhado = table.Column<double>(type: "float", nullable: false),
                    ValorLicitado = table.Column<double>(type: "float", nullable: false),
                    Saldo = table.Column<double>(type: "float", nullable: false),
                    Desconto = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDeBaixa", x => new { x.BaixaID, x.ID, x.ValorUnitario });
                    table.ForeignKey(
                        name: "FK_ItemDeBaixa_BaixaLicitacao_BaixaID",
                        column: x => x.BaixaID,
                        principalTable: "BaixaLicitacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ItemDeEmpenho",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    EmpenhoID = table.Column<int>(type: "int", nullable: false),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemDeBaixa = table.Column<bool>(type: "bit", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    QtdeEmpenhada = table.Column<double>(type: "float", nullable: false),
                    QtdeEntregue = table.Column<double>(type: "float", nullable: false),
                    QtdeAEntregar = table.Column<double>(type: "float", nullable: false),
                    ValorEntregue = table.Column<double>(type: "float", nullable: false),
                    ValorUnitario = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDeEmpenho", x => new { x.EmpenhoID, x.ID });
                    table.ForeignKey(
                        name: "FK_ItemDeEmpenho_Empenho_EmpenhoID",
                        column: x => x.EmpenhoID,
                        principalTable: "Empenho",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemDeNota",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NotaID = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmpenhoID = table.Column<int>(type: "int", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Quantidade = table.Column<double>(type: "float", nullable: false),
                    ValorUnitario = table.Column<double>(type: "float", nullable: false),
                    ValorTotal = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDeNota", x => new { x.NotaID, x.ID });
                    table.ForeignKey(
                        name: "FK_ItemDeNota_Nota_NotaID",
                        column: x => x.NotaID,
                        principalTable: "Nota",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemDeReajuste",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    AtaID = table.Column<int>(type: "int", nullable: false),
                    ReajusteID = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Quantidade = table.Column<double>(type: "float", nullable: false),
                    ValorUnitario = table.Column<double>(type: "float", nullable: false),
                    ValorTotal = table.Column<double>(type: "float", nullable: false),
                    Desconto = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDeReajuste", x => new { x.AtaID, x.ReajusteID, x.ID });
                    table.ForeignKey(
                        name: "FK_ItemDeReajuste_Reajuste_ReajusteID",
                        column: x => x.ReajusteID,
                        principalTable: "Reajuste",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpenhoPolicia_BaixaID",
                table: "EmpenhoPolicia",
                column: "BaixaID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDeReajuste_ReajusteID",
                table: "ItemDeReajuste",
                column: "ReajusteID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpenhoPolicia");

            migrationBuilder.DropTable(
                name: "ItemDeBaixa");

            migrationBuilder.DropTable(
                name: "ItemDeEmpenho");

            migrationBuilder.DropTable(
                name: "ItemDeNota");

            migrationBuilder.DropTable(
                name: "ItemDeReajuste");

            migrationBuilder.DropTable(
                name: "BaixaPolicia");

            migrationBuilder.DropTable(
                name: "BaixaLicitacao");

            migrationBuilder.DropTable(
                name: "Empenho");

            migrationBuilder.DropTable(
                name: "Nota");

            migrationBuilder.DropTable(
                name: "Reajuste");
        }
    }
}
