// AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Data;
using PaulaPresentesWebMVC.Models;
using Microsoft.AspNetCore.Http;

namespace PaulaPresentesWebMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
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
                .FirstOrDefault(p => p.CodigoBarra == codigoBarra);

            // PRODUTO NÃO EXISTE
            if (produto == null)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Produto não encontrado."
                });
            }

            // SEM ESTOQUE
            if (produto.QuantidadeEstoque <= 0)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Produto sem estoque disponível."
                });
            }

            // PRODUTO OK
            return Json(new
            {
                sucesso = true,

                idProduto = produto.IdProduto,

                nome = produto.Nome,

                marca = produto.Marca,

                cor = produto.Cor,

                preco = produto.PrecoVenda ?? 0,

                estoque = produto.QuantidadeEstoque,

                imagem = produto.Imagens?.FirstOrDefault() != null
                ? "/img/produtos/" + produto.Imagens.First().CaminhoImagem
                : "/images/produto.jpg"
            });
        }

        // ============================
        // FINALIZAR VENDA
        // ============================
        [HttpPost]
        public IActionResult FinalizarVenda(
            List<int> idProduto,
            List<int> quantidade,
            decimal subtotal,
            decimal desconto,
            decimal total,
            string formaPagamento,
            string vendedor)
        {
            var venda = new Venda
            {
                DataVenda = DateTime.Now,
                Subtotal = subtotal,
                Desconto = desconto,
                Total = total,
                Funcionario = vendedor
            };

            _context.Venda.Add(venda);
            _context.SaveChanges();

            for (int i = 0; i < idProduto.Count; i++)
            {
                var produto = _context.Produto
                    .FirstOrDefault(p => p.IdProduto == idProduto[i]);

                if (produto != null)
                {
                    // BAIXA ESTOQUE
                    produto.QuantidadeEstoque -= quantidade[i];

                    // AUMENTA VENDIDOS
                    produto.QuantidadeVendida += quantidade[i];

                    // ITEM VENDA
                    var vendaItem = new VendaItem
                    {
                        IdVenda = venda.IdVenda,
                        IdProduto = produto.IdProduto,
                        Quantidade = quantidade[i],
                        PrecoUnitario = produto.PrecoVenda ?? 0,
                        Subtotal =
                            (produto.PrecoVenda ?? 0) * quantidade[i]
                    };

                    _context.VendaItem.Add(vendaItem);
                }
            }

            var entradaCaixa = new MovimentacaoCaixa
            {
                Tipo = "ENTRADA",
                Categoria = "Venda",
                Descricao = $"Venda #{venda.IdVenda} - {vendedor} - {formaPagamento}",
                
                Valor = total,
                DataMovimentacao = DateTime.Now,
            };

            _context.MovimentacaoCaixa.Add(entradaCaixa);

            _context.SaveChanges();

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

            return View(produtos.ToList());
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
            );

            if (codigoExiste)
            {
                ModelState.AddModelError("", "Código de barras já existe.");
            }

            // RETORNA VIEW

            if (!ModelState.IsValid)
            {
                return View(produto);
            }

            // DATA

            produto.DataCompra = DateTime.Now;

            produto.QuantidadeVendida = 0;

            // SALVA PRODUTO

            _context.Produto.Add(produto);

            await _context.SaveChangesAsync();

            // PASTA IMAGENS

            string pastaImagens =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/img/produtos");

            if (!Directory.Exists(pastaImagens))
            {
                Directory.CreateDirectory(pastaImagens);
            }

            // SALVA IMAGENS

            foreach (var imagem in imagens)
            {
                if (imagem.Length > 0)
                {
                    string nomeArquivo =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(imagem.FileName);

                    string caminhoCompleto =
                        Path.Combine(pastaImagens, nomeArquivo);

                    using (var stream =
                        new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await imagem.CopyToAsync(stream);
                    }

                    ProdutoImagem produtoImagem =
                        new ProdutoImagem
                        {
                            IdProduto = produto.IdProduto,
                            CaminhoImagem = nomeArquivo
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
        // =========================================
        [HttpGet]
        public IActionResult MovimentacaoCaixa(DateTime? dataInicial, DateTime? dataFinal)
        {
            var movimentacoes = _context.MovimentacaoCaixa
                .AsQueryable();

            // FILTRO POR DATA
            if (dataInicial.HasValue)
            {
                movimentacoes = movimentacoes.Where(m =>
                    m.DataMovimentacao >= dataInicial.Value);
            }

            if (dataFinal.HasValue)
            {
                movimentacoes = movimentacoes.Where(m =>
                    m.DataMovimentacao <= dataFinal.Value);
            }

            movimentacoes = movimentacoes
                .OrderByDescending(m => m.DataMovimentacao);

            ViewBag.TotalEntradas = movimentacoes
                .Where(m => m.Tipo == "ENTRADA")
                .Sum(m => m.Valor);

            ViewBag.TotalSaidas = movimentacoes
                .Where(m => m.Tipo == "SAIDA")
                .Sum(m => m.Valor);

            ViewBag.Saldo = (decimal)ViewBag.TotalEntradas - (decimal)ViewBag.TotalSaidas;

            return View(movimentacoes.ToList());
        }


        // =========================================
        // NOVA ENTRADA
        // =========================================
        [HttpPost]
        public IActionResult NovaEntrada(MovimentacaoCaixa movimentacao)
        {
            movimentacao.Tipo = "ENTRADA";
            movimentacao.DataMovimentacao = DateTime.Now;

            _context.MovimentacaoCaixa.Add(movimentacao);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult NovaSaida(MovimentacaoCaixa movimentacao)
        {
            movimentacao.Tipo = "SAIDA";
            movimentacao.DataMovimentacao = DateTime.Now;

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
            DateTime inicio = dataInicial ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            DateTime fim = dataFinal ?? DateTime.Now;

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

            DateTime inicioSemana = DateTime.Now.AddDays(-7);

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
                    dataInicial = DateTime.Now.AddMonths(-1);
                }

                if (periodo == "semestre")
                {
                    dataInicial = DateTime.Now.AddMonths(-6);
                }

                if (periodo == "ano")
                {
                    dataInicial = DateTime.Now.AddYears(-1);
                }

                dataFinal = DateTime.Now;
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

            _context.Viagem.Add(viagem);

            // 💸 SAÍDA NO CAIXA (VIAGEM)
            var saidaCaixa = new MovimentacaoCaixa
            {
                Tipo = "SAIDA",
                Categoria = "Viagem",
                Descricao = $"Viagem #{viagem.IdViagem}",
                Valor = viagem.CustoTotal,
                DataMovimentacao = DateTime.Now,
            };

            _context.MovimentacaoCaixa.Add(saidaCaixa);

            _context.SaveChanges();

            TempData["Mensagem"] =
                "Viagem cadastrada com sucesso!";

            return RedirectToAction("Viagem");
        }


        public IActionResult Clientes()
        {
            var clientes = _context.Cliente.ToList();

            var hoje = DateTime.Today;

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
            _context.Cliente.Add(cliente);
            _context.SaveChanges();

            return RedirectToAction("Clientes");
        }

    }
}