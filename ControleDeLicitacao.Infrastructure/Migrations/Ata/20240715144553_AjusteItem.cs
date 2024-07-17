using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeLicitacao.Infrastructure.Migrations.Ata
{
    public partial class AjusteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDeAta",
                table: "ItemDeAta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDeAta",
                table: "ItemDeAta",
                columns: new[] { "AtaID", "ID", "ValorUnitario" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDeAta",
                table: "ItemDeAta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDeAta",
                table: "ItemDeAta",
                columns: new[] { "AtaID", "ID" });
        }
    }
}
