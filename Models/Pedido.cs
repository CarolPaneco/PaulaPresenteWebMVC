using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Pedido
    {
        [Key]
        [Column("id_pedido")]
        public int IdPedido { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("frete")]
        public decimal Frete { get; set; }

        [Column("tipo_frete")]
        public string TipoFrete { get; set; }

        [Column("data_pedido")]
        public DateTime DataPedido { get; set; }
    }
}