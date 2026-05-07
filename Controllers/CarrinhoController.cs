using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Data;
using PaulaPresentesWebMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class CarrinhoController : Controller
{
    private readonly AppDbContext _context;

    public CarrinhoController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // LISTAR CARRINHO
    // =========================
    public IActionResult Index()
    {
        int? clienteId = HttpContext.Session.GetInt32("UsuarioId");

        if (clienteId == null)
            return RedirectToAction("Login", "Conta");

        var carrinho = _context.Carrinho
            .Include(c => c.Itens)
            .ThenInclude(i => i.Produto)
            .ThenInclude(p => p.Imagens)
            .Include(c => c.Itens)
            .ThenInclude(i => i.Produto)
            .ThenInclude(p => p.Estoque)
            .FirstOrDefault(c => c.IdCliente == clienteId);

            if (carrinho == null)
            return View(new List<CarrinhoItem>());


        return View(carrinho.Itens);
    }

    // =========================
    // DTO FRETE
    // =========================
    public class FreteDTO
    {
        public decimal Valor { get; set; }
        public int Prazo { get; set; }
        public string Tipo { get; set; }
    }

    // =========================
    // ADICIONAR PRODUTO
    // =========================
    [HttpPost]
public IActionResult Adicionar(int produtoId, int quantidade)
{
    var clienteId = HttpContext.Session.GetInt32("UsuarioId");

    if (clienteId == null)
    {
        TempData["ReturnUrl"] = $"/Produto/Detalhes/{produtoId}";
        return RedirectToAction("Login", "Conta");
    }

    var produto = _context.Produto
        .Include(p => p.Estoque)
        .FirstOrDefault(p => p.IdProduto == produtoId);

    if (produto == null)
        return NotFound();

    int estoqueDisponivel = produto.Estoque?.Quantidade ?? 0;

    if (estoqueDisponivel <= 0)
    {
        TempData["Erro"] = "Produto esgotado.";
        return RedirectToAction("Detalhes", "Produto", new { id = produtoId });
    }

    var carrinho = _context.Carrinho
        .Include(c => c.Itens)
        .FirstOrDefault(c => c.IdCliente == clienteId);

    if (carrinho == null)
    {
        carrinho = new Carrinho
        {
            IdCliente = clienteId.Value,
            Itens = new List<CarrinhoItem>()
        };

        _context.Carrinho.Add(carrinho);
    }

    var item = carrinho.Itens.FirstOrDefault(i => i.IdProduto == produtoId);

    // 🔥 SE JÁ EXISTE NO CARRINHO
    if (item != null)
    {
        int novaQuantidade = item.Quantidade + quantidade;

        if (novaQuantidade > estoqueDisponivel)
        {
            TempData["Erro"] = $"Você já tem {item.Quantidade} no carrinho. Máximo disponível: {estoqueDisponivel}.";
            return RedirectToAction("Detalhes", "Produto", new { id = produtoId });
        }

        item.Quantidade = novaQuantidade;
    }
    else
    {
        // 🔥 NOVO ITEM
        if (quantidade > estoqueDisponivel)
        {
            TempData["Erro"] = $"Só temos {estoqueDisponivel} unidade(s) em estoque.";
            return RedirectToAction("Detalhes", "Produto", new { id = produtoId });
        }

        carrinho.Itens.Add(new CarrinhoItem
        {
            IdProduto = produtoId,
            Quantidade = quantidade
        });
    }

    _context.SaveChanges();

    return RedirectToAction("Index");
}

    // =========================
    // CALCULAR FRETE
    // =========================
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

        // 🔥 MONTA LISTA DE PRODUTOS
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

        ViewBag.Frete = carrinho.ValorFrete;
        ViewBag.TipoFrete = carrinho.TipoFrete;
        ViewBag.PrazoEntrega = carrinho.PrazoFrete;

        return Content(result, "application/json");
    }

    // =========================
    // SALVAR FRETE
    // =========================
    [HttpPost]
    public IActionResult SalvarFrete([FromBody] FreteDTO frete)
    {
        var clienteId = HttpContext.Session.GetInt32("UsuarioId");

        if (clienteId == null)
            return Unauthorized();

        var carrinho = _context.Carrinho
            .FirstOrDefault(c => c.IdCliente == clienteId);

        if (carrinho == null)
            return BadRequest();

        carrinho.ValorFrete = frete.Valor;
        carrinho.PrazoFrete = frete.Prazo;
        carrinho.TipoFrete = frete.Tipo;

        _context.SaveChanges();

        return Ok();
    }

    // =========================
    // REMOVER ITEM
    // =========================
    [HttpPost]
    public IActionResult Remover(int idItem)
    {
        var item = _context.CarrinhoItem
            .FirstOrDefault(i => i.IdItem == idItem);

        if (item != null)
        {
            _context.CarrinhoItem.Remove(item);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    // =========================
    // ATUALIZAR QUANTIDADE
    // =========================
    [HttpPost]
    public IActionResult AtualizarQuantidade(int idItem, int quantidade)
    {
        var item = _context.CarrinhoItem
            .Include(i => i.Produto)
                .ThenInclude(p => p.Estoque)
            .FirstOrDefault(i => i.IdItem == idItem);

        if (item == null)
            return RedirectToAction("Index");

        var estoque = item.Produto?.Estoque?.Quantidade ?? int.MaxValue;

        if (quantidade > estoque)
        {
            TempData["AvisoEstoque"] = item.IdItem;
            quantidade = estoque;
        }

        if (quantidade < 1)
        {
            _context.CarrinhoItem.Remove(item);
        }
        else
        {
            item.Quantidade = quantidade;
        }

        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}