using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Models;
using PaulaPresentesWebMVC.Data;


/*
Controller de Pedido. Responsável por realizar e administrar pedidos. 
*/
namespace PaulaPresentesWebMVC.Controllers
{
    public class PedidoController : Controller
    {
        private readonly AppDbContext _context;

        public PedidoController(AppDbContext context)
        {
            _context = context;
        }

        //checkout é a tela principal do pedido, aonde confere as informações do pedido antes de finalizar
        public IActionResult Checkout()
        {
            int? clienteId = HttpContext.Session.GetInt32("UsuarioId");

            if (clienteId == null)
                return RedirectToAction("Login", "Conta");

            //pega os dados do carrinho 
            var carrinho = _context.Carrinho
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(p => p.Imagens)
                .FirstOrDefault(c => c.IdCliente == clienteId);

            var pedido = _context.Pedido
                .FirstOrDefault(c => c.IdCliente == clienteId);

            if (carrinho == null || !carrinho.Itens.Any())
                return RedirectToAction("Index", "Carrinho");

            //pega o valor de todos os produtos como subtotal
            decimal subtotal = carrinho.Itens.Sum(i =>
                (i.Produto.PrecoVenda ?? 0) * i.Quantidade);

            decimal valorFrete = pedido.ValorFrete ?? 0;

            //viewbag é como se fosse uma mochila de dados temporária, para guardar informações.
            ViewBag.Itens = carrinho.Itens;
            ViewBag.Subtotal = subtotal;
            ViewBag.Frete = valorFrete;
            ViewBag.Total = subtotal + valorFrete;
            ViewBag.TipoFrete = pedido.TipoFrete;
            ViewBag.PrazoFrete = pedido.PrazoFrete ?? 0;

            return View();
        }

        /*
        [HttpPost]
        //metodo de finalizar a compra. 
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

            var pedidoExistente = _context.Pedido
            .FirstOrDefault(c => c.IdCliente == clienteId);

            decimal frete = pedidoExistente?.ValorFrete ?? 0;

            decimal total = subtotal + frete;

            var novoPedido = new Pedido
            {
                IdCliente = clienteId.Value,
                Subtotal = subtotal,
                ValorFrete = frete,
                Total = total,
                TipoFrete = pedidoExistente?.TipoFrete,
                DataPedido = DateTime.UtcNow
            };

            _context.Pedido.Add(novoPedido);
            _context.SaveChanges();

            _context.CarrinhoItem.RemoveRange(carrinho.Itens);

            _context.SaveChanges();

            //retorna para a tela de sucesso
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
                DataPedido = DateTime.UtcNow
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

                if (cupom.DataValidade.HasValue && cupom.DataValidade < DateTime.UtcNow)
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
        
        public async Task<IActionResult> CalcularFreteMelhorEnvio(string cepDestino)
        {
            var clienteId = HttpContext.Session.GetInt32("UsuarioId");

            if (clienteId == null)
                return Unauthorized();

            var carrinho = _context.Carrinho
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefault(c => c.IdCliente == clienteId);

            if (carrinho == null || !carrinho.Itens.Any())
                return Json(new { erro = "Carrinho vazio" });

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiZjQyOWUwMGY0NzU1MjdhYmVmN2MxYTQ1ZWMyNzFlMjVjNjM2YzQ5ODQ2MjRkYTQzMTkwMGU3YTQ1YmJmM2Y2NDA0ZjFkZTBjNTQ4MGYwNDQiLCJpYXQiOjE3Nzc5MjU5MTEuNTk4Nzc5LCJuYmYiOjE3Nzc5MjU5MTEuNTk4NzgxLCJleHAiOjE4MDk0NjE5MTEuNTg2MjE0LCJzdWIiOiJhMWIzNGJkNy0yNGY0LTRhYTgtOTgzYi01OWY3ODQ5NmJlMmUiLCJzY29wZXMiOlsiZWNvbW1lcmNlLXNoaXBwaW5nIiwic2hpcHBpbmctY2FsY3VsYXRlIiwic2hpcHBpbmctY2hlY2tvdXQiXX0.uzi7kRpWH29j-IHwaG0PeViEW6jmd4U6FvQu7we8Zv_WxC-knAMNyOHxLIBG_dVfNpJ89nbBuDpoT1V1WN1XQQ5yeQNafznoTX1ximmSi-YPdv5nw2Z77pH8Yy-epOra19G5jGpc8ZIHB2e7Pls4QJnrkEZ4Gq5LJr2FCFnAz-EzhGSmlAE29Q7p3wuiMIfMGeEuKimkOx6b8bHJ6G42X1ubgjX7EBexwiuwCE5KbnbCQN42r-w1TafCK0vPLw6UNUPzeXORgtZELcKdYvkZhxpNgjlnQ9msvauwdX0Y3v0oD_dP4AlLFUqoxg-jjKw20LXMONWOqz0Yz0Kj2NYppPTdDyQXEAa2UszsxOrdkKoDkOuHmlUwotcnU-9VTNHk3iqTEdokunDDOlJwqKSV59JU8sX8sEla7V6mCfitFPpJAqiaTBUYlIQNoodKmvbRiEMuFyHWkTak0cFQeD80RaliUxlyIXlVGQM9ev4Huv2bHwt1WzOI_uAw_jCR8WzXtjFR1xPc5LeicXqgW9odQDQ9lhfRRRzefT6JjwgtpbobP91ozHKkEoRA1w0f08twAuBvemvCuGCYiaYFBESX_d-EWljGW96rUSqyjtPqCb7l3gs9X3CNS-GFy0XkYPqMjszYW0js52t--Tq_UIZ29uYIboKqC9nNhkZ9CrMIgj8");

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var produtos = carrinho.Itens.Select(item => new
            {
                id = item.Produto.IdProduto.ToString(),
                width = item.Produto.Largura ?? 15,
                height = item.Produto.Altura ?? 10,
                length = item.Produto.Comprimento ?? 20,
                weight = item.Produto.Peso ?? 1,
                insurance_value = item.Produto.PrecoVenda ?? 0,
                quantity = item.Quantidade
            }).ToArray();

            var body = new
            {
                from = new { postal_code = "15400065" },
                to = new { postal_code = cepDestino },
                products = produtos
            };

            var json = JsonSerializer.Serialize(body);

            var response = await client.PostAsync(
                "https://melhorenvio.com.br/api/v2/me/shipment/calculate",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            var options = JsonSerializer.Deserialize<List<FreteResponse>>(result);
            var sedex = options
                    .Where(x => x.name != null &&
                                x.name.ToUpper().Contains("SEDEX"))
                    .ToList();

            return Content(result, "application/json");
        }*/
    }
}