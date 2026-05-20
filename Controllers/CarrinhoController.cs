using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Data;
using PaulaPresentesWebMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

/*
Controller de Carrinho. 
Ele que vai ser responsável pelas ações do carrinho como:

Consulta no banco de dados. 
Inserir produto no carrinho. 
Retirada de produto no carrinho. 

*/

public class CarrinhoController : Controller
{

    private readonly AppDbContext _context;

    //É aonde puxa as informações de Context
    public CarrinhoController(AppDbContext context)
    {
        _context = context;
    }

    //Página principal do carrinho, aonde se lista os produtos. 
    public IActionResult Index()
    {   
        //A Session é um espaço onde você guarda informações temporárias do usuário enquanto ele está usando o sistema.
        int? clienteId = HttpContext.Session.GetInt32("UsuarioId");

        //Se não estiver logado em nenhuma conta, é jogado para o Controller Login Método Conta
        if (clienteId == null)
            return RedirectToAction("Login", "Conta");


        var carrinho = _context.Carrinho // mesma coisa que SELECT * FROM Carrinho
            .Include(c => c.Itens) // carrega os itens do carrinho 
            .ThenInclude(i => i.Produto) // carega o produto dos itens 
            .ThenInclude(p => p.Imagens) // carrega a imagem do produto
            .FirstOrDefault(c => c.IdCliente == clienteId); // pega o carrinho aonde é o id do cliente é o mesmo do cliente logado

            //caso o cliente não tenha carrinho, criar um novo.            
            if (carrinho == null)
            return View(new List<CarrinhoItem>());

        //envia os itens do carrinho pra View   
        return View(carrinho.Itens);
    }

    //método de adicionar produto ao carrinho 
    [HttpPost] // metodo so pode ser chamado por requisição post 
    //IActionResult significar que vai retornar alguma resposta. 
    public IActionResult Adicionar(int produtoId, int quantidade)   
    {
        var clienteId = HttpContext.Session.GetInt32("UsuarioId");

        if (clienteId == null)
        {
            TempData["ReturnUrl"] = $"/Produto/Detalhes/{produtoId}";
            return RedirectToAction("Login", "Conta");
        }

        //acessa tabela produto
        var produto = _context.Produto
            .FirstOrDefault(p => p.IdProduto == produtoId);

        if (produto == null)
            return NotFound();

        // pega a quantidade disponível da bolsa especifica, se for null dá o valor 0.
        int estoqueDisponivel = produto?.QuantidadeEstoque ?? 0;

        //caso não tenha quantidade disponível consta como produto esgotado. 
        if (estoqueDisponivel <= 0)
        {
            TempData["Erro"] = "Produto esgotado.";
            return RedirectToAction("Detalhes", "Produto", new { id = produtoId });// volta para a página do produto. 
        }

        //pega a tabela de carrinho e seus itens.
        var carrinho = _context.Carrinho
            .Include(c => c.Itens)
            .FirstOrDefault(c => c.IdCliente == clienteId);

        //caso o cliente nao tenha carrinho, cria um novo pra ele e adiciona no banco do Carrinho. 
        if (carrinho == null)
        {
            carrinho = new Carrinho
            {
                IdCliente = clienteId.Value,
                Itens = new List<CarrinhoItem>()
            };

            _context.Carrinho.Add(carrinho);
        }

        //verifica se o produto já existe no carrinho
        var item = carrinho.Itens.FirstOrDefault(i => i.IdProduto == produtoId);

        //se o produto já estava no carrinho.
        if (item != null)
        {   
            //caso já exista, só vai aumentar a quantidade solicitada perante possibilidade. 
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
            //caso não exista ainda no carrinho vai ser um novo produto. 
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

        //redireciona para a página atual do carrinho. 
        return RedirectToAction("Index");
    }


    //metodo de remover item do carrinho
    [HttpPost]
    public IActionResult Remover(int idItem)
    {
        //pega a tabela do item do carrinho, porque ja existe no carrinho. 
        var item = _context.CarrinhoItem
            .FirstOrDefault(i => i.IdItem == idItem);

        //caso exista 
        if (item != null)
        {
            _context.CarrinhoItem.Remove(item);//apaga do banco 
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    //metodo de atualizar a quantidade, seja para menos ou mais. 
    public IActionResult AtualizarQuantidade(int idItem, int quantidade)
    {
        //pega na tabela de item o item específico
        var item = _context.CarrinhoItem
            .Include(i => i.Produto)
            .FirstOrDefault(i => i.IdItem == idItem);

        if (item == null)
            return RedirectToAction("Index");

        //pega o estoque do produto
        var estoque = item.Produto?.QuantidadeEstoque ?? 0;

        //verifica se a quantidade que atualizou é maior ou menor que o estoque
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