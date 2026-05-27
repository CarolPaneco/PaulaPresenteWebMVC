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
        public IActionResult Index(
            string categoria,
            string cor,
            string preco
        )
        {
            var produtos = _context.Produto
                .Include(p => p.Imagens)
                .AsQueryable();

            // CATEGORIA
            if (!string.IsNullOrEmpty(categoria))
            {
                produtos = produtos.Where(p =>
                    p.Categoria == categoria);
            }

            // COR
            if (!string.IsNullOrEmpty(cor))
            {
                produtos = produtos.Where(p =>
                    p.Cor == cor);
            }

            // PREÇO
            if (!string.IsNullOrEmpty(preco))
            {
                switch(preco)
                {
                    case "0-50":
                        produtos = produtos.Where(p =>
                            p.PrecoVenda <= 50);
                        break;

                    case "50-100":
                        produtos = produtos.Where(p =>
                            p.PrecoVenda >= 50
                            &&
                            p.PrecoVenda <= 100);
                        break;

                    case "100-200":
                        produtos = produtos.Where(p =>
                            p.PrecoVenda >= 100
                            &&
                            p.PrecoVenda <= 200);
                        break;

                    case "200-500":
                        produtos = produtos.Where(p =>
                            p.PrecoVenda >= 200
                            &&
                            p.PrecoVenda <= 500);
                        break;

                    case "500+":
                        produtos = produtos.Where(p =>
                            p.PrecoVenda >= 500);
                        break;
                }
            }

            ViewBag.CategoriaSelecionada = categoria;
            ViewBag.CorSelecionada = cor;
            ViewBag.PrecoSelecionado = preco;

            return View(produtos.ToList());
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