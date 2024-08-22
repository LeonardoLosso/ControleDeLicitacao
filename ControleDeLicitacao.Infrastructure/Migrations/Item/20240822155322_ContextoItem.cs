using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Item
{
    public partial class ContextoItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Itens",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EhCesta = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnidadePrimaria = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UnidadeSecundaria = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itens", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItensAssociativos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemPaiID = table.Column<int>(type: "int", nullable: false),
                    ItemFilhoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensAssociativos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItensAssociativos_Itens_ItemFilhoID",
                        column: x => x.ItemFilhoID,
                        principalTable: "Itens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensAssociativos_Itens_ItemPaiID",
                        column: x => x.ItemPaiID,
                        principalTable: "Itens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NomesAssociativos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomesAssociativos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NomesAssociativos_Itens_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Itens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensAssociativos_ItemFilhoID",
                table: "ItensAssociativos",
                column: "ItemFilhoID");

            migrationBuilder.CreateIndex(
                name: "IX_ItensAssociativos_ItemPaiID",
                table: "ItensAssociativos",
                column: "ItemPaiID");

            migrationBuilder.CreateIndex(
                name: "IX_NomesAssociativos_ItemID",
                table: "NomesAssociativos",
                column: "ItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensAssociativos");

            migrationBuilder.DropTable(
                name: "NomesAssociativos");

            migrationBuilder.DropTable(
                name: "Itens");
        }
    }
}
