using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Models;
using PaulaPresentesWebMVC.Data;

namespace PaulaPresentesWebMVC.Controllers
{
    public class PedidoController : Controller
    {
        private readonly AppDbContext _context;

        public PedidoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Checkout()
        {
            int? clienteId = HttpContext.Session.GetInt32("UsuarioId");

            if (clienteId == null)
                return RedirectToAction("Login", "Conta");

            var carrinho = _context.Carrinho
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(p => p.Imagens)
                .FirstOrDefault(c => c.IdCliente == clienteId);

            if (carrinho == null || !carrinho.Itens.Any())
                return RedirectToAction("Index", "Carrinho");

            decimal subtotal = carrinho.Itens.Sum(i =>
                (i.Produto.PrecoVenda ?? 0) * i.Quantidade);

            decimal frete = carrinho.ValorFrete ?? 0;

            ViewBag.Itens = carrinho.Itens;
            ViewBag.Subtotal = subtotal;
            ViewBag.Frete = frete;
            ViewBag.Total = subtotal + frete;
            ViewBag.TipoFrete = carrinho.TipoFrete;
            ViewBag.PrazoFrete = carrinho.PrazoFrete ?? 0;

            return View();
        }

        [HttpPost]
        public IActionResult Finalizar()
        {
            int? clienteId = HttpContext.Session.GetInt32("UsuarioId");

            if (clienteId == null)
                return RedirectToAction("Login", "Conta");

            var carrinho = _context.Carrinho
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefault(c => c.IdCliente == clienteId);

            if (carrinho == null || !carrinho.Itens.Any())
                return RedirectToAction("Index", "Carrinho");

            decimal subtotal = carrinho.Itens.Sum(i =>
                (i.Produto.PrecoVenda ?? 0) * i.Quantidade);

            decimal frete = carrinho.ValorFrete ?? 0;
            decimal total = subtotal + frete;

            var pedido = new Pedido
            {
                IdCliente = clienteId.Value,
                Subtotal = subtotal,
                Frete = frete,
                Total = total,
                TipoFrete = carrinho.TipoFrete,
                DataPedido = DateTime.Now
            };

            _context.Pedido.Add(pedido);
            _context.SaveChanges();

            _context.CarrinhoItem.RemoveRange(carrinho.Itens);
            carrinho.ValorFrete = 0;
            carrinho.TipoFrete = null;

            _context.SaveChanges();

            return RedirectToAction("Sucesso");
        }

        public IActionResult Sucesso()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Finalizar(string Rua, string Numero, string Bairro, string Cidade, string Estado, string CEP)
        {
            int? clienteId = HttpContext.Session.GetInt32("UsuarioId");

            if (clienteId == null)
                return RedirectToAction("Login", "Conta");

            var cliente = _context.Cliente.FirstOrDefault(c => c.IdCliente == clienteId);

            if (cliente == null)
                return RedirectToAction("Login", "Conta");

            // 🔥 SALVAR ENDEREÇO NO CLIENTE
            cliente.Rua = Rua;
            cliente.Numero = Numero;
            cliente.Bairro = Bairro;
            cliente.Cidade = Cidade;
            cliente.Estado = Estado;
            cliente.CEP = CEP;

            _context.SaveChanges();


            var carrinho = _context.Carrinho
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefault(c => c.IdCliente == clienteId);

            if (carrinho == null || !carrinho.Itens.Any())
                return RedirectToAction("Index", "Carrinho");

            decimal subtotal = carrinho.Itens.Sum(i =>
                (i.Produto.PrecoVenda ?? 0) * i.Quantidade);

            decimal frete = carrinho.ValorFrete ?? 0;
            decimal total = subtotal + frete;

            var pedido = new Pedido
            {
                IdCliente = clienteId.Value,
                Subtotal = subtotal,
                Frete = frete,
                Total = total,
                TipoFrete = carrinho.TipoFrete,
                DataPedido = DateTime.Now
            };

            _context.Pedido.Add(pedido);

            // limpar carrinho
            _context.CarrinhoItem.RemoveRange(carrinho.Itens);
            carrinho.ValorFrete = 0;
            carrinho.TipoFrete = null;

            _context.SaveChanges();

            return RedirectToAction("Sucesso");
        }

        [HttpPost]
        public IActionResult AplicarCupom([FromBody] CupomDTO data)
        {
            if (data == null)
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Dados não recebidos"
                });

            if (string.IsNullOrEmpty(data.Codigo))
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Digite um cupom"
                });

            try
            {
                if (string.IsNullOrEmpty(data.Codigo))
                    return Json(new { sucesso = false, mensagem = "Digite um cupom" });

                var cupom = _context.Cupom
                    .FirstOrDefault(c => c.Codigo != null && c.Codigo == data.Codigo);

                if (cupom == null)
                    return Json(new { sucesso = false, mensagem = "Cupom não existe" });

                if (!cupom.Ativo)
                    return Json(new { sucesso = false, mensagem = "Cupom inativo" });

                if (cupom.DataValidade.HasValue && cupom.DataValidade < DateTime.Now)
                    return Json(new { sucesso = false, mensagem = "Cupom expirado" });

                if (data.Total < cupom.ValorMinimo)
                    return Json(new
                    {
                        sucesso = false,
                        mensagem = $"Mínimo de R$ {cupom.ValorMinimo:F2}"
                    });

                decimal desconto = 0;

                if ((cupom.TipoDesconto ?? "") == "Porcentagem")
                {
                    desconto = data.Total * (cupom.Valor / 100);
                }
                else
                {
                    desconto = cupom.Valor;
                }

                decimal totalFinal = data.Total - desconto;

                return Json(new
                {
                    sucesso = true,
                    desconto,
                    totalFinal
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Erro interno",
                    erro = ex.Message
                });
            }


        }
    }
}