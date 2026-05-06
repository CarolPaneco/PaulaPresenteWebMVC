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

        public DbSet<Cupom> Cupom { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Estoque)
                .WithOne(e => e.Produto)
                .HasForeignKey<Estoque>(e => e.IdProduto);
        }
        public DbSet<Estoque> Estoque { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}