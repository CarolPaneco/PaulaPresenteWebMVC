using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class ProdutoImagem
    {
        [Key]
        [Column("id_imagem")]
        public int IdImagem { get; set; }

        [Column("id_produto")]
        public int IdProduto { get; set; }

        [Column("caminho_imagem")]
        public string? CaminhoImagem { get; set; }

        [ForeignKey("IdProduto")]
        public Produto? Produto { get; set; }
    }
}