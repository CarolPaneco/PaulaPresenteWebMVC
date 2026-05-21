using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class PedidoItem
    {
        [Key]
        [Column("id_pedido_item")]
        public int IdPedidoItem { get; set; }

        [Column("id_pedido")]
        public int IdPedido { get; set; }

        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }

        [Column("id_produto")]
        public int IdProduto { get; set; }

        [ForeignKey("IdProduto")]
        public Produto? Produto { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        [Column("subtotal_item")]
        public decimal Subtotal { get; set; }

    }
}