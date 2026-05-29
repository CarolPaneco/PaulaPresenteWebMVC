using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Models;

namespace PaulaPresentesWebMVC.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<ProdutoImagem> ProdutoImagem { get; set; }
        public DbSet<Carrinho> Carrinho { get; set; }
        public DbSet<CarrinhoItem> CarrinhoItem { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<PedidoItem> PedidoItem { get; set; }
        public DbSet<Cupom> Cupom { get; set; }
        public DbSet<Venda> Venda { get; set; }
        public DbSet<VendaItem> VendaItem { get; set; }
        public DbSet<HistoricoCrediario> HistoricoCrediario { get; set; }
        public DbSet<Viagem> Viagem { get; set; }
        public DbSet<MovimentacaoCaixa> MovimentacaoCaixa { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacaoEstoque { get; set; }
        public DbSet<Pagamento> Pagamento { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}