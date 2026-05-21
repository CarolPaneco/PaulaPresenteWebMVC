using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class MovimentacaoEstoque
    {
        [Key]
        [Column("id_movimentacao_estoque")]
        public int IdMovimentacaoEstoque { get; set; }

        [Column("id_produto")]
        public int IdProduto { get; set; }

        [ForeignKey("IdProduto")]
        public Produto? Produto { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("data_movimentacao_estoque")]
        public DateTime DataMovimentacaoEstoque { get; set; }

    }
}