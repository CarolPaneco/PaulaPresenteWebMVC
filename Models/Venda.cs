using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Venda
    {
        [Key]
        [Column("id_venda")]
        public int IdVenda { get; set; }

        [Column("data_venda")]
        public DateTime DataVenda { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("desconto")]
        public decimal Desconto { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("funcionario")]
        public string? Funcionario { get; set; }
    }
}