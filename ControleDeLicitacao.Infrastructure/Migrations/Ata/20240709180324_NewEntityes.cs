using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Ata
{
    public partial class NewEntityes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_ItemDeReajuste_ReajusteID",
                table: "ItemDeReajuste",
                column: "ReajusteID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemDeReajuste");

            migrationBuilder.DropTable(
                name: "Reajuste");
        }
    }
}
