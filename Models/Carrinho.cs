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

        [Column("data_criacao")]
        public DateTime? DataCriacao { get; set; }

        public List<CarrinhoItem>? Itens { get; set; }
    }
}