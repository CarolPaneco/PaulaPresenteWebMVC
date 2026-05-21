using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;


namespace PaulaPresentesWebMVC.Models
{
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("telefone")]
        public string Telefone { get; set; }

        [Column("data_nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataNascimento { get; set; }

        [Column("cpf")]
        public string? CPF { get; set; }

        [Column("rua")]
        public string? Rua { get; set; }

        [Column("numero")]
        public string? Numero { get; set; }

        [Column("bairro")]
        public string? Bairro { get; set; }

        [Column("cidade")]
        public string? Cidade { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("cep")]
        public string? CEP { get; set; }

        [Column("senha")]
        public string? Senha { get; set; }

        [Column("data_cadastro")]
        public DateTime? DataCadastroCliente { get; set; } = DateTime.UtcNow;

        [Column("ativo")]
        public bool? Ativo { get; set; }


    }
}