using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PaulaPresentesWebMVC.Models
{
    public class Cupom
    {
        [Key]
        [Column("id_cupom")]
        public int IdCupom { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; }

        [Column("tipo_desconto")]
        public string TipoDesconto { get; set; }

        [Column("valor")]
        public decimal Valor { get; set; }

        [Column("valor_minimo")]
        public decimal ValorMinimo { get; set; }

        [Column("data_validade")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataValidadeCupom { get; set; }

        [Column("ativo")]
        public bool CupomAtivo { get; set; }

    }
}