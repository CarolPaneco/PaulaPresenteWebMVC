using Microsoft.AspNetCore.Mvc;
using PaulaPresentesWebMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PaulaPresentesWebMVC.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Detalhes(int id)
        {
            var produto = _context.Produto
                .Include(p => p.Imagens)
                .Include(p => p.Estoque)
                .FirstOrDefault(p => p.IdProduto == id);

            if (produto == null)
                return NotFound();

            var variacoes = _context.Produto
                .Include(p => p.Estoque)
                .Where(p => p.Nome == produto.Nome
                        && p.Marca == produto.Marca
                        && p.Estoque != null
                        && p.IdProduto != produto.IdProduto
                        && p.Estoque.Quantidade > 0)
                .ToList();

            ViewBag.Variacoes = variacoes;

            var relacionados = _context.Produto
                .Include(p => p.Imagens)
                .Where(p => p.Cor == produto.Cor
                    && p.Estoque != null
                    && p.Estoque.Quantidade > 0
                    && p.IdProduto != produto.IdProduto)
                    .ToList();

            ViewBag.Relacionados = relacionados;

            return View(produto);
        }
    }
}