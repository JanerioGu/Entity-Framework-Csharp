using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja.Migrations
{
    /// <inheritdoc />
    public partial class AddVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeVendida",
                table: "Vendas",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "PrecoVendaUnitario",
                table: "Vendas",
                newName: "PrecoUnitario");

            migrationBuilder.RenameColumn(
                name: "DataVenda",
                table: "Vendas",
                newName: "Data");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "Vendas",
                newName: "QuantidadeVendida");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "Vendas",
                newName: "PrecoVendaUnitario");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Vendas",
                newName: "DataVenda");
        }
    }
}
