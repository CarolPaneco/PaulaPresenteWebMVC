using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PaulaPresentesWebMVC.Models;
using PaulaPresentesWebMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace PaulaPresentesWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string categoria)
        {
            var produto = _context.Produto
                .Include(p => p.Imagens)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                produto = produto.Where(p => p.Categoria == categoria);
            }

            var lista = produto.ToList();

            ViewBag.CategoriaSelecionada = categoria;

            return View(lista);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Sobre()
        {
            return View();
        }
    }
}