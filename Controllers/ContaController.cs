using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PaulaPresentesWebMVC.Models;
using PaulaPresentesWebMVC.Data;

/*
    Controller da conta aonde acessa login ou cadastro
*/

namespace PaulaPresentesWebMVC.Controllers
{
    public class ContaController : Controller
    {
        private readonly AppDbContext _context;

        public ContaController(AppDbContext context)
        {
            _context = context;
        }

        //chama o método login que chama a view auth 
        public IActionResult Login()
        {
            return View("Auth");
        }


        [HttpPost]
        //formaulario de login 
        public IActionResult Login(string email, string senha)
        {

            //ja setando qual login e senha é o admin
            if (email == "admin@paula.com" && senha == "admin2306")
            {
                HttpContext.Session.SetString("Admin", "true");

                //direciona para a view do controller admim
                return RedirectToAction("Index", "Admin");
            }

            var cliente = _context.Cliente
                .FirstOrDefault(c => c.Email == email && c.Senha == senha);

            if (cliente != null)
            {
                HttpContext.Session.SetString("UsuarioNome", cliente.Nome);
                HttpContext.Session.SetInt32("UsuarioId", cliente.IdCliente);

                return RedirectToAction("Index", "Home");
            }

            //Caso não ache no banco de dados a combinação
            ViewBag.ErroLogin = "Email ou senha inválidos";
            return View("Auth");
        }

        
        public IActionResult Cadastro()
        {
            return View("Auth");
        }


        [HttpPost]
        public IActionResult Cadastro(Cliente cliente)
        {
            // verifica se já existe esse cadastro
            var existe = _context.Cliente
                .FirstOrDefault(c => c.Email == cliente.Email);

            if (existe != null)
            {
                ViewBag.ErroCadastro = "Este email já está cadastrado";
                return View("Auth");
            }


            if (cliente.DataNascimento.HasValue)
            {
                cliente.DataNascimento = DateTime.SpecifyKind(
                    cliente.DataNascimento.Value,
                    DateTimeKind.Utc
                );
            }

            cliente.DataCadastroCliente = DateTime.UtcNow;

            _context.Cliente.Add(cliente);
            _context.SaveChanges();

            ViewBag.Sucesso = "Cadastro realizado com sucesso! Faça login.";

            return View("Auth");
        }

        // caso aperte no perfil e ja exista cadastro ele vai pro perfil 
        public IActionResult Perfil()
        {
            var nome = HttpContext.Session.GetString("UsuarioNome");

            if (nome == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        //logout 
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}