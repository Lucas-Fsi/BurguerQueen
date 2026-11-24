using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;
using BurguerQueen.Models.ViewModels;
using BurguerQueen.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace BurguerQueen.Controllers;

// responsável APENAS por orquestrar o fluxo HTTP
// (receber requisições, chamar o repositório e devolver a View ou redirecionamento)

public class ProdutosController : Controller
{
    // Dependência da interface não da classe em si: facilita manutenção
    private readonly IProdutoRepository _produtoRepository;

    // Injeção de Dependência o injeta automaticamente o ProdutoRepository
    // configurado via AddScoped no Program.cs
    public ProdutosController(IProdutoRepository produtoRepository)
    {
        this._produtoRepository = produtoRepository;
    }

    // GET: /Produtos ou /Produtos?tipo=1 (Lanche) / /Produtos?tipo=2 (Bebida)
    // Lista todos os produtos ou filtra por tipo, usando a coleção fortemente tipada do repositório
    public async Task<IActionResult> Index(TipoProduto? tipo = null)
    {
        try
        {
            var todosProdutos = (await this._produtoRepository.ObterTodosAsync()).ToList();

            var produtos = tipo.HasValue
                ? await this._produtoRepository.ObterPorTipoAsync(tipo.Value)
                : todosProdutos;

            // Guarda o filtro atual e os totais para a View exibir KPIs e destacar a aba selecionada.
            ViewBag.TipoSelecionado = tipo;
            ViewBag.TotalProdutos = todosProdutos.Count;
            ViewBag.TotalLanches = todosProdutos.Count(p => p.Tipo == TipoProduto.Lanche);
            ViewBag.TotalBebidas = todosProdutos.Count(p => p.Tipo == TipoProduto.Bebida);

            return View(produtos);
        }
        catch (Exception)
        {
            // Evita tela de erro 500 e exibe erro amigável caso o banco ainda não esteja criado
            TempData["Erro"] = "Não foi possível carregar os produtos no momento.";
            ViewBag.TotalProdutos = 0;
            ViewBag.TotalLanches = 0;
            ViewBag.TotalBebidas = 0;
            return View(Enumerable.Empty<BurguerQueen.Models.Produto>());
        }
    }

    // GET: /Produtos/Details/5
    // Exibe os detalhes completos de um produto específico
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var produto = await this._produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
            {
                TempData["Erro"] = "Produto não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }
        catch (Exception)
        {
            TempData["Erro"] = "Ocorreu um erro ao buscar os detalhes do produto.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Produtos/Create
    // Abre a tela com o formulário vazio de cadastro usando o ProdutoViewModel
    public IActionResult Create()
    {
        return View(new ProdutoViewModel());
    }

    // POST: /Produtos/Create
    // Recebe os dados enviados pelo formulário de cadastro
    [HttpPost]
    [ValidateAntiForgeryToken] // Proteção nativa do ASP.NET contra ataques CSRF (falsificação de requisição)
    public async Task<IActionResult> Create(ProdutoViewModel viewModel)
    {
        // Valida as Data Annotations do ViewModel (Requisito de ModelState)
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            // Converte o ViewModel para Lanche ou Bebida
            // Se alguma regra de domínio for violada, o construtor da entidade lança DominioException
            var produto = viewModel.ParaEntidade();

            await this._produtoRepository.AdicionarAsync(produto);

            // TempData sobrevive a um Redirect e serve para exibir alerta amigável de sucesso na tela Index
            TempData["Sucesso"] = $"Produto \"{produto.Nome}\" cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (DominioException ex)
        {
            // Tratamento de exceção de negócio
            // captura o erro do domínio e devolve a mensagem amigável para o formulário, sem estourar erro 500
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(viewModel);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao cadastrar o produto.");
            return View(viewModel);
        }
    }

    // GET: /Produtos/Edit/5
    // Carrega o produto do banco e preenche o ProdutoViewModel para exibir no formulário de edição
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var produto = await this._produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
            {
                TempData["Erro"] = "Produto não encontrado para edição.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = ProdutoViewModel.DeEntidade(produto);
            return View(viewModel);
        }
        catch (Exception)
        {
            TempData["Erro"] = "Ocorreu um erro ao carregar o produto para edição.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Produtos/Edit/5
    // Salva as alterações enviadas pelo formulário de edição
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProdutoViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            TempData["Erro"] = "Identificador do produto inválido.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            var produto = viewModel.ParaEntidade();
            await this._produtoRepository.AtualizarAsync(produto);

            TempData["Sucesso"] = $"Produto \"{produto.Nome}\" atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (DominioException ex)
        {
            // Captura tanto falhas de validação do domínio quanto "Produto não encontrado" vindo do Repository
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(viewModel);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao atualizar o produto.");
            return View(viewModel);
        }
    }

    // GET: /Produtos/Delete/5
    // Abre a tela de confirmação perguntando se o atendente realmente deseja excluir o produto
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var produto = await this._produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
            {
                TempData["Erro"] = "Produto não encontrado para exclusão.";
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }
        catch (Exception)
        {
            TempData["Erro"] = "Ocorreu um erro ao carregar o produto para exclusão.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Produtos/Delete/5
    // ActionName("Delete") permite que a rota continue sendo /Produtos/Delete mesmo com nome de método diferente,
    // pois em C# não podemos ter dois métodos 'Delete(int id)' com a mesma assinatura na mesma classe
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await this._produtoRepository.RemoverAsync(id);
            TempData["Sucesso"] = "Produto excluído com sucesso!";
        }
        catch (DominioException ex)
        {
            TempData["Erro"] = ex.Message;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            // Código 19 no SQLite indica violação de restrição (ex: Chave Estrangeira).
            // Evita erro 500 se o atendente tentar excluir um produto que já está vinculado a um ItemPedido.
            TempData["Erro"] = "Não é possível excluir este produto porque ele já está vinculado a um pedido.";
        }
        catch (Exception)
        {
            TempData["Erro"] = "Ocorreu um erro inesperado ao excluir o produto.";
        }

        return RedirectToAction(nameof(Index));
    }
}