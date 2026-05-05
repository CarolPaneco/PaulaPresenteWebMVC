using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Estoque
    {
        [Key]
        [Column("id_estoque")]
        public int IdEstoque { get; set; }

        [Column("id_produto")]
        public int IdProduto { get; set; }
        public Produto Produto { get; set; }

        [Column("quantidade")]
        public int Quantidade  { get; set; }
    }
}