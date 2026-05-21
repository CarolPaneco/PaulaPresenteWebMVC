using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class CarrinhoItem
    {
        [Key]
        [Column("id_item")]
        public int IdItem { get; set; }

        [Column("id_carrinho")]
        public int IdCarrinho { get; set; }

        [Column("id_produto")]
        public int IdProduto { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        [Column("subtotal_item")]
        public decimal SubtotalItem { get; set; }

        [ForeignKey("IdCarrinho")]
        public Carrinho? Carrinho { get; set; }

        [ForeignKey("IdProduto")]
        public Produto? Produto { get; set; }
    }
}