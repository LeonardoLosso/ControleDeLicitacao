using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Ata
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtaLicitacao",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Edital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Unidade = table.Column<int>(type: "int", nullable: false),
                    EmpresaID = table.Column<int>(type: "int", nullable: false),
                    OrgaoID = table.Column<int>(type: "int", nullable: false),
                    DataLicitacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Vigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalLicitado = table.Column<double>(type: "float", nullable: false),
                    TotalReajustes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtaLicitacao", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemDeAta",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    AtaID = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Quantidade = table.Column<double>(type: "float", nullable: false),
                    ValorUnitario = table.Column<double>(type: "float", nullable: false),
                    ValorTotal = table.Column<double>(type: "float", nullable: false),
                    Desconto = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDeAta", x => new { x.AtaID, x.ID });
                    table.ForeignKey(
                        name: "FK_ItemDeAta_AtaLicitacao_AtaID",
                        column: x => x.AtaID,
                        principalTable: "AtaLicitacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemDeAta");

            migrationBuilder.DropTable(
                name: "AtaLicitacao");
        }
    }
}
