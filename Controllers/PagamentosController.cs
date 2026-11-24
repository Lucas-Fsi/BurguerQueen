using BurguerQueen.Exceptions;
using BurguerQueen.Models;
using BurguerQueen.Models.ViewModels;
using BurguerQueen.Repositories.Interfaces;
using BurguerQueen.Services;
using Microsoft.AspNetCore.Mvc;

namespace BurguerQueen.Controllers;

// O controller só orquestra: as regras ficam nas entidades e no PagamentoService.
public class PagamentosController : Controller
{
    private readonly IPagamentoRepository _pagamentos;
    private readonly PagamentoService _service;

    public PagamentosController(IPagamentoRepository pagamentos, PagamentoService service)
    {
        _pagamentos = pagamentos;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int pedidoId)
    {
        try
        {
            var pedido = await _service.ObterPedidoPagavelAsync(pedidoId);
            return View(new PagamentoViewModel { PedidoId = pedido.Id, ValorTotal = pedido.Total });
        }
        catch (DominioException ex)
        {
            TempData["Erro"] = ex.Message;
            return RedirectToAction("Details", "Pedidos", new { id = pedidoId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PagamentoViewModel vm)
    {
        try
        {
            // O pedido é consultado de novo: o valor cobrado nunca vem do formulário
            var pedido = await _service.ObterPedidoPagavelAsync(vm.PedidoId);
            vm.ValorTotal = pedido.Total;

            if (ModelState.IsValid)
            {
                var pagamento = vm.ParaEntidade(pedido.Id, pedido.Total);

                // Os dados da transação (código de autorização, chave etc.) já foram
                // informados, então o pagamento é registrado como aprovado.
                pagamento.Confirmar();

                var id = await _pagamentos.AdicionarAsync(pagamento);
                TempData["Sucesso"] = "Pagamento registrado com sucesso.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        catch (DominioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var pagamento = await _pagamentos.ObterPorIdAsync(id);
        if (pagamento is null)
            return NotFound();

        return View(PagamentoDetalhesViewModel.DeEntidade(pagamento));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Confirmar(int id)
        => AlterarStatus(id, p => p.Confirmar(), "Pagamento confirmado.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Cancelar(int id)
        => AlterarStatus(id, p => p.Cancelar(), "Pagamento cancelado.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Estornar(int id)
        => AlterarStatus(id, p => p.Estornar(), "Pagamento estornado.");

    // Carrega, aplica a regra do domínio, salva e volta para os detalhes
    private async Task<IActionResult> AlterarStatus(int id, Action<Pagamento> acao, string mensagemSucesso)
    {
        var pagamento = await _pagamentos.ObterPorIdAsync(id);
        if (pagamento is null)
            return NotFound();

        try
        {
            acao(pagamento);
            await _pagamentos.AtualizarStatusAsync(pagamento);
            TempData["Sucesso"] = mensagemSucesso;
        }
        catch (DominioException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
