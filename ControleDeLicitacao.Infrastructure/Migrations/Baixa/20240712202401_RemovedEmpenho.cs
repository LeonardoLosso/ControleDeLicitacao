using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class RemovedEmpenho : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDeBaixa",
                table: "ItemDeBaixa");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDeBaixa",
                table: "ItemDeBaixa",
                columns: new[] { "BaixaID", "ID", "ValorUnitario" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDeBaixa",
                table: "ItemDeBaixa");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDeBaixa",
                table: "ItemDeBaixa",
                columns: new[] { "BaixaID", "ID" });
        }
    }
}
