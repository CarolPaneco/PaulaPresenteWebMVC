// AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Data;
using PaulaPresentesWebMVC.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http.Headers;

namespace PaulaPresentesWebMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController( AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ============================
        // TELA REGISTRAR VENDA
        // ============================
        public IActionResult RegistrarVenda()
        {
            ViewBag.Clientes = _context.Cliente.ToList();
            return View();
        }

        // ============================
        // BUSCAR PRODUTO PELO CÓDIGO
        // ============================
        [HttpGet]
        public IActionResult BuscarProduto(string codigoBarra)
        {
            var produto = _context.Produto
                .Include(p => p.Imagens)
                .Where(p =>
                    p.CodigoBarra == codigoBarra
                    &&
                    p.QuantidadeEstoque > 0
                )
                .ToList();

            // PRODUTO NÃO EXISTE
            if (produto == null)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Produto não encontrado."
                });
            }

            if (!produto.Any())
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Produto sem estoque disponível."
                });
            }

            var produtoPrincipal = produto
            .OrderByDescending(p => p.QuantidadeEstoque)
            .First();

            // PRODUTO OK
            return Json(new
            {
                sucesso = true,

                idProduto = produtoPrincipal.IdProduto,

                nome = produtoPrincipal.Nome,

                marca = produtoPrincipal.Marca,

                cor = produtoPrincipal.Cor,

                preco = produtoPrincipal.PrecoVenda ?? 0,

                estoque = produtoPrincipal.QuantidadeEstoque,

                imagem = produtoPrincipal.Imagens?.FirstOrDefault() != null
                ? produtoPrincipal.Imagens.First().CaminhoImagem
                : "/images/produto.jpg",

                coresDisponiveis = produto.Select(p => new
                {
                    idProduto = p.IdProduto,

                    cor = p.Cor,

                    marca = p.Marca,

                    estoque = p.QuantidadeEstoque,

                    preco = p.PrecoVenda ?? 0,

                    imagem = p.Imagens?.FirstOrDefault() != null
                        ? p.Imagens.First().CaminhoImagem
                        : "/images/produto.jpg"
                })
            });
        }

        // ============================
        // FINALIZAR VENDA
        // ============================
        [HttpPost]
        public IActionResult FinalizarVenda(
            List<int> idProduto,
            List<int> quantidade,
            string subtotal,
            string desconto,
            string total,
            string formaPagamento,
            string vendedor,
            int? clienteId)
        {
            // =========================
            // CONVERSÕES
            // =========================

            decimal subtotalConvertido =
                decimal.Parse(subtotal, CultureInfo.InvariantCulture);

            decimal descontoConvertido =
                decimal.Parse(desconto, CultureInfo.InvariantCulture);

            decimal totalConvertido =
                decimal.Parse(total, CultureInfo.InvariantCulture);

            // =========================
            // CRIAR VENDA
            // =========================

            var venda = new Venda
            {
                DataVenda = DateTime.UtcNow,
                Subtotal = subtotalConvertido,
                Desconto = descontoConvertido,
                Total = totalConvertido,
                FormaPagamento = formaPagamento,
                Funcionario = vendedor,
                IdCliente = clienteId
            };

            _context.Venda.Add(venda);

            // Salva primeiro para gerar IdVenda
            _context.SaveChanges();

            // =========================
            // CREDIÁRIO
            // =========================

            if (formaPagamento == "Crediario" && clienteId.HasValue)
            {
                var cliente = _context.Cliente
                    .FirstOrDefault(c => c.IdCliente == clienteId.Value);

                if (cliente != null)
                {
                    cliente.ValorDevido += totalConvertido;
                }
            }

            // =========================
            // ITENS DA VENDA
            // =========================

            for (int i = 0; i < idProduto.Count; i++)
            {
                var produto = _context.Produto
                    .FirstOrDefault(p => p.IdProduto == idProduto[i]);

                if (produto == null)
                    continue;

                // Atualiza estoque
                produto.QuantidadeEstoque -= quantidade[i];

                // Atualiza quantidade vendida
                produto.QuantidadeVendida += quantidade[i];

                // Cria item da venda
                var vendaItem = new VendaItem
                {
                    IdVenda = venda.IdVenda,
                    IdProduto = produto.IdProduto,
                    Quantidade = quantidade[i],
                    PrecoUnitario = produto.PrecoVenda ?? 0,
                    Subtotal = (produto.PrecoVenda ?? 0) * quantidade[i]
                };

                _context.VendaItem.Add(vendaItem);
            }

            // =========================
            // MOVIMENTAÇÃO DE CAIXA
            // =========================

            var entradaCaixa = new MovimentacaoCaixa
            {
                Tipo = "ENTRADA",
                Categoria = "Venda",
                Descricao =
                    $"Venda #{venda.IdVenda} - {vendedor} - {formaPagamento}",

                Valor = totalConvertido,
                DataMovimentacao = DateTime.UtcNow
            };

            _context.MovimentacaoCaixa.Add(entradaCaixa);

            // =========================
            // HISTÓRICO RECENTE
            // =========================

            var historico = _context.MovimentacaoCaixa
                .OrderByDescending(m => m.DataMovimentacao)
                .Take(3)
                .ToList();

            ViewBag.Historico = historico;

            // =========================
            // SALVAR ALTERAÇÕES
            // =========================

            _context.SaveChanges();

            // =========================
            // REDIRECIONAR
            // =========================

            return RedirectToAction("RegistrarVenda");
        }

        public IActionResult Produtos()
        {
            return View();
        }

        // =========================
        // CONSULTAR PRODUTOS
        // =========================
        public IActionResult ConsultarProdutos(string pesquisa)
        {
            var produtos = _context.Produto
                .Include(p => p.Imagens)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisa))
            {
                produtos = produtos.Where(p =>
                    p.CodigoBarra.Contains(pesquisa)
                    || p.Cor.Contains(pesquisa)
                    || p.Categoria.Contains(pesquisa)
                    || p.Nome.Contains(pesquisa)
                );
            }

            produtos = produtos.OrderByDescending(p => p.IdProduto);

            return View(produtos.ToList());
        }

        [HttpPost]
        public IActionResult Editar(Produto produto, string SenhaAdmin)
        {
            if (SenhaAdmin != "ritinha")
            {
                TempData["Erro"] = "Senha de administrador incorreta!";
                return RedirectToAction("ConsultarProdutos");
            }
        
            var p = _context.Produto
                .FirstOrDefault(x => x.IdProduto == produto.IdProduto);
        
            if (p == null)
                return NotFound();
        
            // Verifica se já existe OUTRO produto com o mesmo código de barras
            // e com quantidade em estoque maior que 0
        var codigoExistente = _context.Produto.Any(x =>
            x.CodigoBarra == produto.CodigoBarra
            &&
            x.IdProduto != produto.IdProduto
            &&
            x.QuantidadeEstoque > 0
            &&
            (
                x.Nome != p.Nome
                ||
                x.Marca != p.Marca
            )
        );

        if (codigoExistente)
        {
            TempData["Erro"] = "Já existe um produto diferente usando esse código de barras!";
            
            return RedirectToAction("ConsultarProdutos");
        }
        
            p.Nome = produto.Nome;
            p.PrecoVenda = produto.PrecoVenda;
            p.PrecoCusto = produto.PrecoCusto;
            p.QuantidadeEstoque = produto.QuantidadeEstoque;
            p.CodigoBarra = produto.CodigoBarra;
        
            _context.SaveChanges();
        
            TempData["Sucesso"] = "Produto atualizado com sucesso!";
            return RedirectToAction("ConsultarProdutos");
        }
        public IActionResult Excluir(int id, string SenhaAdmin)
        {
            if (SenhaAdmin != "ritinha")
            {
                TempData["Erro"] = "Senha de administrador incorreta!";
                return RedirectToAction("ConsultarProdutos");
            }

            var produto = _context.Produto.FirstOrDefault(x => x.IdProduto == id);

            if (produto != null)
            {
                _context.Produto.Remove(produto);
                _context.SaveChanges();
            }

            return RedirectToAction("ConsultarProdutos");
        }

        // =========================
        // CONSULTAR ESTOQUE
        // =========================
        public IActionResult ConsultarEstoque(string pesquisa)
        {
            var produtos = _context.Produto
                .Where(p => p.QuantidadeEstoque > 0)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisa))
            {
                produtos = produtos.Where(p =>

                    p.CodigoBarra.Contains(pesquisa)

                    || p.Cor.Contains(pesquisa)

                    || p.Categoria.Contains(pesquisa)

                    || p.Nome.Contains(pesquisa)
                );
            }

            return View(produtos.ToList());
        }

        // =========================
        // NOVO PRODUTO
        // =========================
        [HttpGet]
        public IActionResult NovoProduto()
        {
            return View();
        }

        // =========================
        // SALVAR PRODUTO
        // =========================

        [HttpPost]
        public async Task<IActionResult> NovoProduto(
            Produto produto,
            List<IFormFile> imagens)
        {
            // VALIDAÇÕES

            if (string.IsNullOrWhiteSpace(produto.Nome))
            {
                ModelState.AddModelError("", "Informe o nome.");
            }

            if (string.IsNullOrWhiteSpace(produto.Marca))
            {
                ModelState.AddModelError("", "Informe a marca.");
            }

            if (string.IsNullOrWhiteSpace(produto.Categoria))
            {
                ModelState.AddModelError("", "Selecione a categoria.");
            }

            if (string.IsNullOrWhiteSpace(produto.Cor))
            {
                ModelState.AddModelError("", "Selecione a cor.");
            }

            if (string.IsNullOrWhiteSpace(produto.CodigoBarra))
            {
                ModelState.AddModelError("", "Informe o código de barras.");
            }

            if (produto.PrecoVenda == null || produto.PrecoVenda <= 0)
            {
                ModelState.AddModelError("", "Informe o preço de venda.");
            }

            if (produto.PrecoCusto == null || produto.PrecoCusto <= 0)
            {
                ModelState.AddModelError("", "Informe o preço de custo.");
            }

            if (produto.QuantidadeEstoque == null || produto.QuantidadeEstoque <= 0)
            {
                ModelState.AddModelError("", "Informe o estoque.");
            }

            // IMAGENS

            if (imagens == null || imagens.Count == 0)
            {
                ModelState.AddModelError("", "Adicione pelo menos 1 imagem.");
            }

            if (imagens.Count > 5)
            {
                ModelState.AddModelError("", "Máximo 5 imagens.");
            }

            // CODIGO DUPLICADO

            bool codigoExiste = _context.Produto
            .Any(p =>
                p.CodigoBarra == produto.CodigoBarra
                &&
                p.QuantidadeEstoque > 0
                &&
                (
                    p.Nome != produto.Nome
                    ||
                    p.Marca != produto.Marca
                    ||
                    p.PrecoVenda != produto.PrecoVenda
                )
            );

            if (codigoExiste)
            {
                ModelState.AddModelError("", "Código de barras já pertence a outro produto.");
            }

            // RETORNA VIEW

            if (!ModelState.IsValid)
            {
                return View(produto);
            }

            // DATA

            produto.DataCompra = DateTime.UtcNow;

            produto.QuantidadeVendida = 0;

            // SALVA PRODUTO

            _context.Produto.Add(produto);

            await _context.SaveChangesAsync();

            // SUPABASE

            var url = _configuration["Supabase:Url"];

            var key = _configuration["Supabase:Key"];

            

            // SALVA IMAGENS NO STORAGE

            foreach (var imagem in imagens)
            {
                if (imagem.Length > 0)
                {
                    string nomeArquivo =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(imagem.FileName);

                    using var httpClient = new HttpClient();

                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", key);

                    httpClient.DefaultRequestHeaders.Add(
                        "apikey",
                        key
                    );

                    using var form =
                        new MultipartFormDataContent();

                    using var stream =
                        imagem.OpenReadStream();

                    using var streamContent =
                        new StreamContent(stream);

                    streamContent.Headers.ContentType =
                        new MediaTypeHeaderValue(imagem.ContentType);

                    form.Add(
                        streamContent,
                        "file",
                        nomeArquivo
                    );

                    var response =
                        await httpClient.PostAsync(
                            $"{url}/storage/v1/object/produtos/{nomeArquivo}",
                            form
                        );

                    if (!response.IsSuccessStatusCode)
                    {
                        string erro =
                            await response.Content.ReadAsStringAsync();

                        throw new Exception(erro);
                    }

                    string urlImagem =
                        $"{url}/storage/v1/object/public/produtos/{nomeArquivo}";

                    ProdutoImagem produtoImagem =
                        new ProdutoImagem
                        {
                            IdProduto = produto.IdProduto,
                            CaminhoImagem = urlImagem
                        };

                    _context.ProdutoImagem.Add(produtoImagem);
                }
            }
            await _context.SaveChangesAsync();

            TempData["Mensagem"] =
                "Produto cadastrado com sucesso!";

            return RedirectToAction("ConsultarProdutos");
        }

        // =========================================
        // TELA PRINCIPAL
        [HttpGet]
        public IActionResult MovimentacaoCaixa(DateTime? dataInicial, DateTime? dataFinal)
        {
            var movimentacoes = _context.MovimentacaoCaixa
                .AsQueryable();

            if (dataInicial.HasValue)
            {
                var inicioUtc = DateTime.SpecifyKind(
                    dataInicial.Value,
                    DateTimeKind.Utc);

                movimentacoes = movimentacoes.Where(m =>
                    m.DataMovimentacao >= inicioUtc);
            }

            if (dataFinal.HasValue)
            {
                var finalUtc = DateTime.SpecifyKind(
                    dataFinal.Value,
                    DateTimeKind.Utc);

                movimentacoes = movimentacoes.Where(m =>
                    m.DataMovimentacao <= finalUtc);
            }

            movimentacoes = movimentacoes
                .OrderByDescending(m => m.DataMovimentacao);

            ViewBag.TotalEntradas = movimentacoes
                .Where(m => m.Tipo == "ENTRADA")
                .Sum(m => m.Valor);

            ViewBag.TotalSaidas = movimentacoes
                .Where(m => m.Tipo == "SAIDA")
                .Sum(m => m.Valor);

            ViewBag.Saldo =
                (decimal)ViewBag.TotalEntradas
                - (decimal)ViewBag.TotalSaidas;

            return View(movimentacoes.ToList());
        }


        // =========================================
        // NOVA ENTRADA
        // =========================================
        [HttpPost]
        public IActionResult NovaEntrada(MovimentacaoCaixa movimentacao)
        {
            movimentacao.Tipo = "ENTRADA";
            movimentacao.DataMovimentacao = DateTime.UtcNow;

            _context.MovimentacaoCaixa.Add(movimentacao);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult NovaSaida(MovimentacaoCaixa movimentacao)
        {
            movimentacao.Tipo = "SAIDA";
            movimentacao.DataMovimentacao = DateTime.UtcNow;

            // 🔒 proteção contra NULL (evita crash futuro)
            movimentacao.Categoria ??= "Outros";
            movimentacao.Descricao ??= "Sem descrição";

            _context.MovimentacaoCaixa.Add(movimentacao);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult RelatorioFinanceiro(DateTime? dataInicial, DateTime? dataFinal)
        {
            // PERÍODO PADRÃO = MÊS ATUAL
            DateTime inicio = (dataInicial ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
                .ToUniversalTime();

            DateTime fim = (dataFinal ?? DateTime.UtcNow)
                .ToUniversalTime();

            // =========================
            // VENDAS
            // =========================

            var vendas = _context.Venda
                .Where(v =>
                    v.DataVenda >= inicio &&
                    v.DataVenda <= fim)
                .ToList();

            decimal faturamento = vendas.Sum(v => v.Total);

            decimal descontos = vendas.Sum(v => v.Desconto);

            decimal ticketMedio = 0;

            if (vendas.Count > 0)
            {
                ticketMedio = faturamento / vendas.Count;
            }

            // =========================
            // MOVIMENTAÇÃO CAIXA
            // =========================

            var movimentacoes = _context.MovimentacaoCaixa
                .Where(m =>
                    m.DataMovimentacao >= inicio &&
                    m.DataMovimentacao <= fim)
                .ToList();

            decimal entradas = movimentacoes
                .Where(m => m.Tipo == "ENTRADA")
                .Sum(m => m.Valor);

            decimal saidas = movimentacoes
                .Where(m => m.Tipo == "SAIDA")
                .Sum(m => m.Valor);

            decimal lucro = faturamento - saidas;

            // =========================
            // GASTOS POR CATEGORIA
            // =========================

            var gastosCategoria = movimentacoes
                .Where(m => m.Tipo == "SAIDA")
                .GroupBy(m => m.Categoria)
                .Select(g => new
                {
                    Categoria = g.Key,
                    Valor = g.Sum(x => x.Valor)
                })
                .OrderByDescending(g => g.Valor)
                .ToList();

            // =========================
            // VENDAS DA SEMANA
            // =========================

            DateTime inicioSemana = DateTime.UtcNow.AddDays(-7);

            decimal vendasSemana = _context.Venda
                .Where(v => v.DataVenda >= inicioSemana)
                .Sum(v => v.Total);

            decimal gastosSemana = _context.MovimentacaoCaixa
                .Where(m =>
                    m.Tipo == "SAIDA" &&
                    m.DataMovimentacao >= inicioSemana)
                .Sum(m => m.Valor);

            decimal lucroSemana = vendasSemana - gastosSemana;

            // =========================
            // PRODUTOS MAIS VENDIDOS
            // =========================

            var produtosMaisVendidos = _context.Produto
                .OrderByDescending(p => p.QuantidadeVendida)
                .Take(5)
                .ToList();

            // =========================
            // ESTOQUE BAIXO
            // =========================

            var estoqueBaixo = _context.Produto
                .Where(p => p.QuantidadeEstoque <= 2)
                .OrderBy(p => p.QuantidadeEstoque)
                .ToList();

            // =========================
            // VIEWBAG
            // =========================

            ViewBag.Faturamento = faturamento;

            ViewBag.Entradas = entradas;

            ViewBag.Saidas = saidas;

            ViewBag.Lucro = lucro;

            ViewBag.Descontos = descontos;

            ViewBag.TicketMedio = ticketMedio;

            ViewBag.VendasSemana = vendasSemana;

            ViewBag.GastosSemana = gastosSemana;

            ViewBag.LucroSemana = lucroSemana;

            ViewBag.GastosCategoria = gastosCategoria;

            ViewBag.ProdutosMaisVendidos = produtosMaisVendidos;

            ViewBag.EstoqueBaixo = estoqueBaixo;

            ViewBag.DataInicial = inicio.ToString("yyyy-MM-dd");

            ViewBag.DataFinal = fim.ToString("yyyy-MM-dd");

            return View();
        }

        // =========================================
        // LISTAR VIAGENS
        // =========================================

        [HttpGet]
        public IActionResult Viagem(
            DateTime? dataInicial,
            DateTime? dataFinal,
            string periodo)
        {
            var viagens = _context.Viagem.AsQueryable();

            // =========================
            // FILTRO AUTOMÁTICO
            // =========================

            if (!string.IsNullOrEmpty(periodo))
            {
                if (periodo == "mes")
                {
                    dataInicial = DateTime.UtcNow.AddMonths(-1);
                }

                if (periodo == "semestre")
                {
                    dataInicial = DateTime.UtcNow.AddMonths(-6);
                }

                if (periodo == "ano")
                {
                    dataInicial = DateTime.UtcNow.AddYears(-1);
                }

                dataFinal = DateTime.UtcNow;
            }

            // =========================
            // FILTRO DATA INICIAL
            // =========================

            if (dataInicial.HasValue)
            {
                viagens = viagens.Where(v =>
                    v.DataViagem >= dataInicial.Value);
            }

            // =========================
            // FILTRO DATA FINAL
            // =========================

            if (dataFinal.HasValue)
            {
                viagens = viagens.Where(v =>
                    v.DataViagem <= dataFinal.Value);
            }

            // =========================
            // LISTA FINAL
            // =========================

            var lista = viagens
                .OrderByDescending(v => v.DataViagem)
                .ToList();

            // =========================
            // DASHBOARDS
            // =========================

            ViewBag.TotalViagens = lista.Count;

            ViewBag.TotalGastos = lista.Sum(v => v.CustoTotal);

            ViewBag.TotalCompras = lista.Sum(v => v.ValorTotalCompra);

            ViewBag.MediaViagem =
                lista.Count > 0
                ? lista.Average(v => v.CustoTotal)
                : 0;

            return View(lista);
        }

        // =========================================
        // NOVA VIAGEM
        // =========================================

        [HttpPost]
        public IActionResult NovaViagem(Viagem viagem)
        {
            viagem.CustoTotal =
                viagem.CustoIdaVolta +
                viagem.CustoAlimentacao +
                viagem.ValorTotalCompra;

                viagem.DataViagem = DateTime.SpecifyKind(
                viagem.DataViagem,
                DateTimeKind.Utc
            );

            _context.Viagem.Add(viagem);

            // 💸 SAÍDA NO CAIXA (VIAGEM)
            var saidaCaixa = new MovimentacaoCaixa
            {
                Tipo = "SAIDA",
                Categoria = "Viagem",
                Descricao = $"Viagem #{viagem.IdViagem}",
                Valor = viagem.CustoTotal,
                DataMovimentacao = DateTime.UtcNow,
            };

            _context.MovimentacaoCaixa.Add(saidaCaixa);

            _context.SaveChanges();

            TempData["Mensagem"] =
                "Viagem cadastrada com sucesso!";

            return RedirectToAction("Viagem");
        }

        [HttpGet]
        public IActionResult Clientes()
        {
            var clientes = _context.Cliente.ToList();

            var hoje = DateTime.UtcNow;

            ViewBag.Aniversariantes = clientes
                .Where(c =>
                    c.DataNascimento.HasValue &&
                    c.DataNascimento.Value.Day == hoje.Day &&
                    c.DataNascimento.Value.Month == hoje.Month
                )
                .ToList();

            return View(clientes);
        }

        [HttpPost]
        public IActionResult Clientes(Cliente cliente)
        {
            if (cliente.DataNascimento.HasValue)
            {
                cliente.DataNascimento = DateTime.SpecifyKind(
                    cliente.DataNascimento.Value,
                    DateTimeKind.Utc
                );
            }

            _context.Cliente.Add(cliente);
            _context.SaveChanges();

            return RedirectToAction("Clientes");
        }

        [HttpPost]
        public IActionResult Abater(
            int idCliente,
            decimal valor,
            string formaPagamento)
        {
            // =========================
            // BUSCAR CLIENTE
            // =========================

            var cliente = _context.Cliente
                .FirstOrDefault(c => c.IdCliente == idCliente);

            if (cliente == null)
            {
                TempData["Erro"] = "Cliente não encontrado.";

                return RedirectToAction("Clientes");
            }

            // =========================
            // VALIDAR VALOR
            // =========================

            if (valor <= 0)
            {
                TempData["Erro"] = "Valor inválido.";

                return RedirectToAction("Index");
            }

            if (valor > cliente.ValorDevido)
            {
                TempData["Erro"] =
                    "O valor não pode ser maior que a dívida.";

                return RedirectToAction("Index");
            }

            // =========================
            // ABATER VALOR
            // =========================

            cliente.ValorDevido -= valor;

            // Evita negativo
            if (cliente.ValorDevido < 0)
            {
                cliente.ValorDevido = 0;
            }

            // =========================
            // MOVIMENTAÇÃO DE CAIXA
            // =========================

            var entradaCaixa = new MovimentacaoCaixa
            {
                Tipo = "ENTRADA",

                Categoria = "Crediario",

                Descricao =
                    $"Pagamento crediário - {cliente.Nome} - {formaPagamento}",

                Valor = valor,

                DataMovimentacao = DateTime.UtcNow
            };

            _context.MovimentacaoCaixa.Add(entradaCaixa);

            // =========================
            // SALVAR
            // =========================

            _context.SaveChanges();

            TempData["Sucesso"] =
                "Pagamento realizado com sucesso.";

            return RedirectToAction("Clientes");
        }

        [HttpPost]
        public IActionResult Create(
            string nome,
            string telefone,
            DateTime? dataNascimento)
        {
            // =========================
            // VALIDAÇÕES
            // =========================

            if (string.IsNullOrWhiteSpace(nome))
            {
                TempData["Erro"] = "O nome é obrigatório.";

                return RedirectToAction("RegistrarVenda");
            }

            if (string.IsNullOrWhiteSpace(telefone))
            {
                TempData["Erro"] = "O telefone é obrigatório.";

                return RedirectToAction("RegistrarVenda");
            }

            DateTime? dataNascimentoUtc = dataNascimento.HasValue
            ? DateTime.SpecifyKind(dataNascimento.Value.Date, DateTimeKind.Utc)
            : null;

            if (dataNascimento.HasValue)
            {
                dataNascimentoUtc = DateTime.SpecifyKind(
                    dataNascimento.Value,
                    DateTimeKind.Utc
                );
            }

            // =========================
            // CRIAR CLIENTE
            // =========================

            var cliente = new Cliente
            {
                Nome = nome,
                Telefone = telefone,
                DataNascimento = dataNascimentoUtc,
                ValorDevido = 0
            };

            _context.Cliente.Add(cliente);

            _context.SaveChanges();

            TempData["Sucesso"] =
                "Cliente cadastrado com sucesso.";

            return RedirectToAction("RegistrarVenda");
        }

    }
    
}
