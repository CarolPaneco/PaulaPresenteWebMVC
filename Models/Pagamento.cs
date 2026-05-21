using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Pagamento
    {
        [Key]
        [Column("id_pagamento")]
        public int IdPagamento { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("categoria")]
        public string Categoria { get; set; }

        [Column("valor")]
        public decimal Valor { get; set; }

        [Column("id_pedido")]
        public int? IdPedido { get; set; }

        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }

        [Column("id_venda")]
        public int? IdVenda { get; set; }

        [ForeignKey("IdVenda")]
        public Venda? Venda { get; set; }

    }
}