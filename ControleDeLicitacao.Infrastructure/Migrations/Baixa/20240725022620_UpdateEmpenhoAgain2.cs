using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class UpdateEmpenhoAgain2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDeEmpenho",
                table: "ItemDeEmpenho");

            migrationBuilder.RenameColumn(
                name: "itemDeBaixa",
                table: "ItemDeEmpenho",
                newName: "ItemDeBaixa");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDeEmpenho",
                table: "ItemDeEmpenho",
                columns: new[] { "EmpenhoID", "ID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDeEmpenho",
                table: "ItemDeEmpenho");

            migrationBuilder.RenameColumn(
                name: "ItemDeBaixa",
                table: "ItemDeEmpenho",
                newName: "itemDeBaixa");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDeEmpenho",
                table: "ItemDeEmpenho",
                columns: new[] { "EmpenhoID", "ValorUnitario", "ID" });
        }
    }
}
