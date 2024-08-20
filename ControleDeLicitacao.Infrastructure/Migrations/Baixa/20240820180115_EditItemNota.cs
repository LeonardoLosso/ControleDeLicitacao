using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class EditItemNota : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "QtdeCaixa",
                table: "ItemDeNota",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ValorCaixa",
                table: "ItemDeNota",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QtdeCaixa",
                table: "ItemDeNota");

            migrationBuilder.DropColumn(
                name: "ValorCaixa",
                table: "ItemDeNota");
        }
    }
}
