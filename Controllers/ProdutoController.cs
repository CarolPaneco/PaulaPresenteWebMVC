using Microsoft.AspNetCore.Mvc;
using PaulaPresentesWebMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// controller do produto, aonde tem mais detalhes e pode selecionar ele para levar ao carrinho

namespace PaulaPresentesWebMVC.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // tela de detalhes
        public IActionResult Detalhes(int id)
        {
            // pega as informações do banco de dados 
            var produto = _context.Produto
                .Include(p => p.Imagens)
                .FirstOrDefault(p => p.IdProduto == id);

            if (produto == null)
                return NotFound();
            
            // pega as outras opções de cores do mesmo produto
            var variacoes = _context.Produto
                .Where(p => p.Nome == produto.Nome
                        && p.Marca == produto.Marca
                        && p.IdProduto != produto.IdProduto
                        && p.QuantidadeEstoque > 0)
                .ToList();

            ViewBag.Variacoes = variacoes;

            // pega os produtos da mesma cor do produto
            var relacionados = _context.Produto
                .Include(p => p.Imagens)
                .Where(p => p.Cor == produto.Cor
                    && p.QuantidadeEstoque > 0
                    && p.IdProduto != produto.IdProduto)
                    .ToList();

            ViewBag.Relacionados = relacionados;

            return View(produto);
        }
    }
}