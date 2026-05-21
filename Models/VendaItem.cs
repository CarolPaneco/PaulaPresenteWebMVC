using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class VendaItem
    {
        [Key]
        [Column("id_venda_item")]
        public int IdVendaItem { get; set; }

        [Column("id_venda")]
        public int IdVenda { get; set; }

        [ForeignKey("IdVenda")]
        public Venda? Venda { get; set; }

        [Column("id_produto")]
        public int IdProduto { get; set; }

        [ForeignKey("IdProduto")]
        public Produto? Produto { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

    }
}