using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PaulaPresentesWebMVC.Models;
using PaulaPresentesWebMVC.Data;

namespace PaulaPresentesWebMVC.Controllers
{
    public class ContaController : Controller
    {
        private readonly AppDbContext _context;

        public ContaController(AppDbContext context)
        {
            _context = context;
        }

        // 🔓 GET LOGIN
        public IActionResult Login()
        {
            return View("Auth");
        }

        // 🔐 POST LOGIN
        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            var cliente = _context.Cliente
                .FirstOrDefault(c => c.Email == email && c.Senha == senha);

            if (cliente != null)
            {
                HttpContext.Session.SetString("UsuarioNome", cliente.Nome);
                HttpContext.Session.SetInt32("UsuarioId", cliente.IdCliente);

                return RedirectToAction("Index", "Home");
            }

            // ❌ ERRO LOGIN
            ViewBag.ErroLogin = "Email ou senha inválidos";
            return View("Auth"); // 🔥 IMPORTANTE
        }

        // 📝 GET CADASTRO
        public IActionResult Cadastro()
        {
            return View("Auth");
        }

        // 📝 POST CADASTRO
        [HttpPost]
        public IActionResult Cadastro(Cliente cliente)
        {
            var existe = _context.Cliente
                .FirstOrDefault(c => c.Email == cliente.Email);

            if (existe != null)
            {
                // ❌ ERRO CADASTRO
                ViewBag.ErroCadastro = "Este email já está cadastrado";
                return View("Auth"); // 🔥 IMPORTANTE
            }

            _context.Cliente.Add(cliente);
            _context.SaveChanges();

            // ✅ SUCESSO
            ViewBag.Sucesso = "Cadastro realizado com sucesso! Faça login.";

            return View("Auth"); // 🔥 IMPORTANTE
        }

        // 👤 PERFIL
        public IActionResult Perfil()
        {
            var nome = HttpContext.Session.GetString("UsuarioNome");

            if (nome == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // 🚪 LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}