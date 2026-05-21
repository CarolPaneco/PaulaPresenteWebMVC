using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaulaPresentesWebMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    telefone = table.Column<string>(type: "text", nullable: false),
                    data_nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cpf = table.Column<string>(type: "text", nullable: true),
                    rua = table.Column<string>(type: "text", nullable: true),
                    numero = table.Column<string>(type: "text", nullable: true),
                    bairro = table.Column<string>(type: "text", nullable: true),
                    cidade = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: true),
                    cep = table.Column<string>(type: "text", nullable: true),
                    senha = table.Column<string>(type: "text", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.id_cliente);
                });

            migrationBuilder.CreateTable(
                name: "Cupom",
                columns: table => new
                {
                    id_cupom = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "text", nullable: false),
                    tipo_desconto = table.Column<string>(type: "text", nullable: false),
                    valor = table.Column<decimal>(type: "numeric", nullable: false),
                    valor_minimo = table.Column<decimal>(type: "numeric", nullable: false),
                    data_validade = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cupom", x => x.id_cupom);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacaoCaixa",
                columns: table => new
                {
                    id_movimentacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    categoria = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false),
                    valor = table.Column<decimal>(type: "numeric", nullable: false),
                    data_movimentacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacaoCaixa", x => x.id_movimentacao);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    id_produto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    categoria = table.Column<string>(type: "text", nullable: true),
                    cor = table.Column<string>(type: "text", nullable: true),
                    preco_custo = table.Column<decimal>(type: "numeric", nullable: true),
                    preco_venda = table.Column<decimal>(type: "numeric", nullable: true),
                    marca = table.Column<string>(type: "text", nullable: true),
                    nome = table.Column<string>(type: "text", nullable: true),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    peso = table.Column<decimal>(type: "numeric", nullable: true),
                    altura = table.Column<decimal>(type: "numeric", nullable: true),
                    largura = table.Column<decimal>(type: "numeric", nullable: true),
                    comprimento = table.Column<decimal>(type: "numeric", nullable: true),
                    quantidade_estoque = table.Column<int>(type: "integer", nullable: true),
                    quantidade_vendida = table.Column<int>(type: "integer", nullable: true),
                    codigo_barra = table.Column<string>(type: "text", nullable: true),
                    data_compra = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.id_produto);
                });

            migrationBuilder.CreateTable(
                name: "Venda",
                columns: table => new
                {
                    id_venda = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_venda = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    desconto = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    funcionario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venda", x => x.id_venda);
                });

            migrationBuilder.CreateTable(
                name: "Viagem",
                columns: table => new
                {
                    id_viagem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_viagem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    custo_ida_volta = table.Column<decimal>(type: "numeric", nullable: false),
                    custo_alimentacao = table.Column<decimal>(type: "numeric", nullable: false),
                    valor_total_compra = table.Column<decimal>(type: "numeric", nullable: false),
                    custo_total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viagem", x => x.id_viagem);
                });

            migrationBuilder.CreateTable(
                name: "Carrinho",
                columns: table => new
                {
                    id_carrinho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrinho", x => x.id_carrinho);
                    table.ForeignKey(
                        name: "FK_Carrinho_Cliente_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    id_pedido = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    desconto = table.Column<decimal>(type: "numeric", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    forma_pagamento = table.Column<string>(type: "text", nullable: false),
                    codigo_rastreio = table.Column<string>(type: "text", nullable: true),
                    tipo_frete = table.Column<string>(type: "text", nullable: false),
                    valor_frete = table.Column<decimal>(type: "numeric", nullable: true),
                    prazo_frete = table.Column<int>(type: "integer", nullable: true),
                    data_pedido = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.id_pedido);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacaoEstoque",
                columns: table => new
                {
                    id_movimentacao_estoque = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_produto = table.Column<int>(type: "integer", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    data_movimentacao_estoque = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacaoEstoque", x => x.id_movimentacao_estoque);
                    table.ForeignKey(
                        name: "FK_MovimentacaoEstoque_Produto_id_produto",
                        column: x => x.id_produto,
                        principalTable: "Produto",
                        principalColumn: "id_produto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoImagem",
                columns: table => new
                {
                    id_imagem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_produto = table.Column<int>(type: "integer", nullable: false),
                    caminho_imagem = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoImagem", x => x.id_imagem);
                    table.ForeignKey(
                        name: "FK_ProdutoImagem_Produto_id_produto",
                        column: x => x.id_produto,
                        principalTable: "Produto",
                        principalColumn: "id_produto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendaItem",
                columns: table => new
                {
                    id_venda_item = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_venda = table.Column<int>(type: "integer", nullable: false),
                    id_produto = table.Column<int>(type: "integer", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    preco_unitario = table.Column<decimal>(type: "numeric", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendaItem", x => x.id_venda_item);
                    table.ForeignKey(
                        name: "FK_VendaItem_Produto_id_produto",
                        column: x => x.id_produto,
                        principalTable: "Produto",
                        principalColumn: "id_produto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendaItem_Venda_id_venda",
                        column: x => x.id_venda,
                        principalTable: "Venda",
                        principalColumn: "id_venda",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarrinhoItem",
                columns: table => new
                {
                    id_item = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_carrinho = table.Column<int>(type: "integer", nullable: false),
                    id_produto = table.Column<int>(type: "integer", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    preco_unitario = table.Column<decimal>(type: "numeric", nullable: false),
                    subtotal_item = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrinhoItem", x => x.id_item);
                    table.ForeignKey(
                        name: "FK_CarrinhoItem_Carrinho_id_carrinho",
                        column: x => x.id_carrinho,
                        principalTable: "Carrinho",
                        principalColumn: "id_carrinho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarrinhoItem_Produto_id_produto",
                        column: x => x.id_produto,
                        principalTable: "Produto",
                        principalColumn: "id_produto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagamento",
                columns: table => new
                {
                    id_pagamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    categoria = table.Column<string>(type: "text", nullable: false),
                    valor = table.Column<decimal>(type: "numeric", nullable: false),
                    id_pedido = table.Column<int>(type: "integer", nullable: true),
                    id_venda = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamento", x => x.id_pagamento);
                    table.ForeignKey(
                        name: "FK_Pagamento_Pedido_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "Pedido",
                        principalColumn: "id_pedido");
                    table.ForeignKey(
                        name: "FK_Pagamento_Venda_id_venda",
                        column: x => x.id_venda,
                        principalTable: "Venda",
                        principalColumn: "id_venda");
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    id_pedido_item = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pedido = table.Column<int>(type: "integer", nullable: false),
                    id_produto = table.Column<int>(type: "integer", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    preco_unitario = table.Column<decimal>(type: "numeric", nullable: false),
                    subtotal_item = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItem", x => x.id_pedido_item);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Pedido_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "Pedido",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Produto_id_produto",
                        column: x => x.id_produto,
                        principalTable: "Produto",
                        principalColumn: "id_produto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carrinho_id_cliente",
                table: "Carrinho",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItem_id_carrinho",
                table: "CarrinhoItem",
                column: "id_carrinho");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItem_id_produto",
                table: "CarrinhoItem",
                column: "id_produto");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacaoEstoque_id_produto",
                table: "MovimentacaoEstoque",
                column: "id_produto");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_id_pedido",
                table: "Pagamento",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_id_venda",
                table: "Pagamento",
                column: "id_venda");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_id_cliente",
                table: "Pedido",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_id_pedido",
                table: "PedidoItem",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_id_produto",
                table: "PedidoItem",
                column: "id_produto");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoImagem_id_produto",
                table: "ProdutoImagem",
                column: "id_produto");

            migrationBuilder.CreateIndex(
                name: "IX_VendaItem_id_produto",
                table: "VendaItem",
                column: "id_produto");

            migrationBuilder.CreateIndex(
                name: "IX_VendaItem_id_venda",
                table: "VendaItem",
                column: "id_venda");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarrinhoItem");

            migrationBuilder.DropTable(
                name: "Cupom");

            migrationBuilder.DropTable(
                name: "MovimentacaoCaixa");

            migrationBuilder.DropTable(
                name: "MovimentacaoEstoque");

            migrationBuilder.DropTable(
                name: "Pagamento");

            migrationBuilder.DropTable(
                name: "PedidoItem");

            migrationBuilder.DropTable(
                name: "ProdutoImagem");

            migrationBuilder.DropTable(
                name: "VendaItem");

            migrationBuilder.DropTable(
                name: "Viagem");

            migrationBuilder.DropTable(
                name: "Carrinho");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Venda");

            migrationBuilder.DropTable(
                name: "Cliente");
        }
    }
}
