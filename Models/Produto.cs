using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Produto
    {
        [Key]
        [Column("id_produto")]
        public int IdProduto { get; set; }

        [Column("categoria")]
        public string? Categoria { get; set; }

        [Column("cor")]
        public string? Cor { get; set; }

        [Column("preco_custo")]
        public decimal? PrecoCusto { get; set; }

        [Column("preco_venda")]
        public decimal? PrecoVenda { get; set; }

        [Column("marca")]
        public string? Marca { get; set; }

        [Column("nome")]
        public string? Nome { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("peso")]
        public decimal? Peso { get; set; }

        [Column("altura")]
        public decimal? Altura { get; set; }

        [Column("largura")]
        public decimal? Largura { get; set; }

        [Column("comprimento")]
        public decimal? Comprimento { get; set; }

        [Column("quantidade_estoque")]
        public int? QuantidadeEstoque { get; set; }

        [Column("quantidade_vendida")]
        public int? QuantidadeVendida { get; set; }

        [Column("codigo_barra")]
        public string? CodigoBarra { get; set; }

        [Column("data_compra")]
        public DateTime? DataCompra { get; set; }

        public List<ProdutoImagem>? Imagens { get; set; }
    }
}