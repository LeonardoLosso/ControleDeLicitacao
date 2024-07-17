using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Baixa
{
    public partial class AdicaoEmpenho : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrgaoID",
                table: "Empenho",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Unidade",
                table: "Empenho",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemDeEmpenho",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    EmpenhoID = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<double>(type: "float", nullable: false),
                    BaixaID = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    QtdeEmpenhada = table.Column<double>(type: "float", nullable: false),
                    QtdeEntregue = table.Column<double>(type: "float", nullable: false),
                    QtdeAEntregar = table.Column<double>(type: "float", nullable: false),
                    ValorEntregue = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDeEmpenho", x => new { x.EmpenhoID, x.ValorUnitario, x.ID });
                    table.ForeignKey(
                        name: "FK_ItemDeEmpenho_Empenho_EmpenhoID",
                        column: x => x.EmpenhoID,
                        principalTable: "Empenho",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemDeEmpenho");

            migrationBuilder.DropColumn(
                name: "OrgaoID",
                table: "Empenho");

            migrationBuilder.DropColumn(
                name: "Unidade",
                table: "Empenho");
        }
    }
}
