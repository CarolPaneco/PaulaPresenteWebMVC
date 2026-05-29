using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaulaPresentesWebMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddCrediarioVendaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "forma_pagamento",
                table: "Venda",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_cliente",
                table: "Venda",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "valor_devido",
                table: "Cliente",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Venda_id_cliente",
                table: "Venda",
                column: "id_cliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Venda_Cliente_id_cliente",
                table: "Venda",
                column: "id_cliente",
                principalTable: "Cliente",
                principalColumn: "id_cliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venda_Cliente_id_cliente",
                table: "Venda");

            migrationBuilder.DropIndex(
                name: "IX_Venda_id_cliente",
                table: "Venda");

            migrationBuilder.DropColumn(
                name: "forma_pagamento",
                table: "Venda");

            migrationBuilder.DropColumn(
                name: "id_cliente",
                table: "Venda");

            migrationBuilder.DropColumn(
                name: "valor_devido",
                table: "Cliente");
        }
    }
}
