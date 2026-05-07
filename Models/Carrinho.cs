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

        [Column("valor_frete")]
        public decimal? ValorFrete { get; set; }

        [Column("prazo_frete")]
        public int? PrazoFrete { get; set; }

        [Column("tipo_frete")]
        public string? TipoFrete { get; set; }

        [Column("prazo_entrega")]
        public string? PrazoEntrega { get; set; }

        [Column("codigo_cupom")]
        public string? CodigoCupom { get; set; }

        [Column("desconto")]
        public decimal? Desconto { get; set; }

        public List<CarrinhoItem>? Itens { get; set; }
    }
}