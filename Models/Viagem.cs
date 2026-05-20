using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Viagem
    {
        [Key]
        [Column("id_viagem")]
        public int IdViagem { get; set; }

        [Column("data_viagem")]
        public DateTime DataViagem { get; set; }

        [Column("custo_ida_volta")]
        public decimal CustoIdaVolta { get; set; }

        [Column("custo_alimentacao")]
        public decimal CustoAlimentacao { get; set; }

        [Column("valor_total_compra")]
        public decimal ValorTotalCompra { get; set; }

        [Column("custo_total")]
        public decimal CustoTotal { get; set; }

    }
}