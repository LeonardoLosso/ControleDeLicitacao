using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class CriacaoNotas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumEmpenho",
                table: "Empenho",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

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
                name: "ItemDeNota",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NotaID = table.Column<int>(type: "int", nullable: false),
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemDeNota");

            migrationBuilder.DropTable(
                name: "Nota");

            migrationBuilder.DropColumn(
                name: "NumEmpenho",
                table: "Empenho");
        }
    }
}
