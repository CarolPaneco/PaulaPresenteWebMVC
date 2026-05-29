using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class HistoricoCrediario
    {
        [Key]
        public int Id { get; set; }

        public int IdCliente { get; set; }

        public string Tipo { get; set; }
        // VENDA ou PAGAMENTO

        public decimal Valor { get; set; }

        public string FormaPagamento { get; set; }

        public string Descricao { get; set; }

        public string Funcionario { get; set; }

        public DateTime Data { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }
    }
}