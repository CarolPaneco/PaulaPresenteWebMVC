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
        public DateTime? DataValidade { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }

    }

    public class CupomDTO
    {
        public string Codigo { get; set; }
        public decimal Total { get; set; }
    }
}