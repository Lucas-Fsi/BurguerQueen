using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BurguerQueen.Exceptions;
using BurguerQueen.Models;
using BurguerQueen.Models.Enums;
using BurguerQueen.Models.ViewModels;
using BurguerQueen.Repositories.Interfaces;
using BurguerQueen.Services;

namespace BurguerQueen.Controllers;

public class PedidosController : Controller
{
    private readonly IPedidoRepository _pedidos;
    private readonly IProdutoRepository _produtos;
    private readonly IPagamentoRepository _pagamentos;
    private readonly IClienteRepository _clientes;
    private readonly PedidoService _pedidoService;

    public PedidosController(
        IPedidoRepository pedidos,
        IProdutoRepository produtos,
        IPagamentoRepository pagamentos,
        IClienteRepository clientes,
        PedidoService pedidoService)
    {
        _pedidos = pedidos;
        _produtos = produtos;
        _pagamentos = pagamentos;
        _clientes = clientes;
        _pedidoService = pedidoService;
    }

    public async Task<IActionResult> Index(StatusPedido? status)
    {
        ViewBag.StatusSelecionado = status;
        return View(await _pedidos.ListarAsync(status));
    }

    public async Task<IActionResult> Create()
        => View(await PreencherClientes(new PedidoCreateViewModel()));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarPedido(PedidoCreateViewModel vm)
    {
        // ClienteId nulo = pedido de balcão
        if (vm.ClienteId.HasValue && await _clientes.ObterPorIdAsync(vm.ClienteId.Value) is null)
        {
            TempData["Erro"] = "Cliente não encontrado.";
            return RedirectToAction(nameof(Create));
        }

        var id = await _pedidos.CriarAsync(new Pedido(vm.ClienteId));
        TempData["Sucesso"] = "Pedido aberto! Agora adicione os itens.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var pedido = await _pedidos.ObterPorIdAsync(id);
        if (pedido is null) return NotFound();

        return View(await MontarDetalhes(pedido));
    }

    // As ações abaixo passam pelo PedidoService, porque as regras dependem dos pagamentos do pedido
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarItem(int id, int produtoId, int quantidade)
    {
        var produto = await _produtos.ObterPorIdAsync(produtoId);
        if (produto is null)
        {
            TempData["Erro"] = "Selecione um produto válido.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return await ExecutarServico(id,
            () => _pedidoService.AdicionarItemAsync(id, produto.Id, produto.Nome, produto.Preco, quantidade),
            "Item adicionado ao pedido.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> RemoverItem(int id, int produtoId)
        => ExecutarServico(id, () => _pedidoService.RemoverItemAsync(id, produtoId), "Item removido do pedido.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> AvancarStatus(int id)
        => ExecutarServico(id, () => _pedidoService.AvancarStatusAsync(id), "Status do pedido atualizado.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Cancelar(int id)
        => ExecutarServico(id, () => _pedidoService.CancelarAsync(id), "Pedido cancelado.");

    // Executa a operação do serviço, trata DominioException em um só lugar e volta para o pedido
    private async Task<IActionResult> ExecutarServico(int id, Func<Task> operacao, string mensagemSucesso)
    {
        try
        {
            await operacao();
            TempData["Sucesso"] = mensagemSucesso;
        }
        catch (DominioException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<PedidoCreateViewModel> PreencherClientes(PedidoCreateViewModel vm)
    {
        var clientes = await _clientes.ObterTodosAsync();
        vm.Clientes = clientes
            .Select(c => new SelectListItem($"{c.Nome} ({c.CPF})", c.Id.ToString()))
            .ToList();
        return vm;
    }

    // ÚNICO ponto que depende do nome do método da IRepository<T>.
    private Task<IEnumerable<Produto>> ListarProdutos()
        => _produtos.ObterTodosAsync();

    private async Task<PedidoDetalhesViewModel> MontarDetalhes(Pedido pedido)
    {
        var produtos = await ListarProdutos();
        var pagamentos = (await _pagamentos.ObterPorPedidoAsync(pedido.Id)).ToList();
        var temPagamentoAprovado = pagamentos.Any(p => p.Status == StatusPagamento.Aprovado);

        var nomeCliente = "Balcão";
        if (pedido.ClienteId.HasValue)
        {
            var cliente = await _clientes.ObterPorIdAsync(pedido.ClienteId.Value);
            if (cliente is not null) nomeCliente = cliente.Nome;
        }

        return new PedidoDetalhesViewModel
        {
            Id = pedido.Id,
            NomeCliente = nomeCliente,
            Status = pedido.Status,
            DataCriacao = pedido.DataCriacao,
            Total = pedido.Total,
            Itens = pedido.Itens.Select(i => new ItemPedidoViewModel
            {
                ProdutoId = i.ProdutoId,
                NomeProduto = i.NomeProduto,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList(),
            Produtos = produtos
                .Select(p => new SelectListItem($"{p.Nome} - {p.Preco:C}", p.Id.ToString()))
                .ToList(),
            Pagamentos = pagamentos.Select(PagamentoDetalhesViewModel.DeEntidade).ToList(),
            TemPagamentoAprovado = temPagamentoAprovado,
            // Mesmas condições do PagamentoService.ObterPedidoPagavelAsync
            PodePagar = pedido.Status != StatusPedido.Cancelado
                        && pedido.Total > 0
                        && !temPagamentoAprovado
        };
    }
}