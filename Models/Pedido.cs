using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class Pedido
    {
        [Key]
        [Column("id_pedido")]
        public int IdPedido { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente? Cliente { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("desconto")]
        public decimal? Desconto { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("forma_pagamento")]
        public string FormaPagamento { get; set; }

        [Column("codigo_rastreio")]
        public string? CodigoRastreio { get; set; }

        [Column("tipo_frete")]
        public string TipoFrete { get; set; }

        [Column("valor_frete")]
        public decimal? ValorFrete { get; set; }

        [Column("prazo_frete")]
        public int? PrazoFrete { get; set; }

        [Column("data_pedido")]
        public DateTime Data { get; set; }
    }
}