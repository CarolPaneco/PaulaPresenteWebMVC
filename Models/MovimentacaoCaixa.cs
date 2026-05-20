using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class MovimentacaoCaixa
    {
        [Key]
        [Column("id_movimentacao")]
        public int IdMovimentacaoCaixa { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("categoria")]
        public string Categoria { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("valor")]
        public decimal Valor { get; set; }

        [Column("data_movimentacao")]
        public DateTime DataMovimentacao { get; set; }


    }
}