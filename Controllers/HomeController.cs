using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PaulaPresentesWebMVC.Models;
using PaulaPresentesWebMVC.Data;
using Microsoft.EntityFrameworkCore;

// Controller do home aonde esta a pagina principal e funções principais 


namespace PaulaPresentesWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // tela principaç
        public IActionResult Index(string categoria)
        {
            var produto = _context.Produto
                .Include(p => p.Imagens)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                produto = produto.Where(p => p.Categoria == categoria);
            }

            // 🔥 ORDENAR PELO MAIS RECENTE
            produto = produto.OrderByDescending(p => p.DataCompra);

            var lista = produto.ToList();

            ViewBag.CategoriaSelecionada = categoria;

            return View(lista);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Categoria(string categoria)
        {
            var produtos = _context.Produto

                .Include(p => p.Imagens)

                .Where(p => p.Categoria == categoria)

                .OrderByDescending(p => p.IdProduto)

                .ToList();

            ViewBag.Categoria = categoria;

            return View(produtos);
        }

        public IActionResult Sobre()
        {
            return View();
        }
    }
}