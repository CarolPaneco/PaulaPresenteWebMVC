using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Carrinho
    {
        [Key]
        [Column("id_carrinho")]
        public int IdCarrinho { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente? Cliente { get; set; }

        [Column("data_criacao")]
        public DateTime? DataCriacaoCarrinho { get; set; } = DateTime.UtcNow;

        public List<CarrinhoItem>? Itens { get; set; }
    }
}