using BurguerQueen.Exceptions;
using BurguerQueen.Models;
using BurguerQueen.Models.Enums;
using BurguerQueen.Repositories.Interfaces;

namespace BurguerQueen.Services;

// Regras que envolvem Pedido e Pagamento juntos. Nenhuma das duas entidades
// conhece a outra, então a verificação fica aqui e não no controller.
public class PagamentoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IPagamentoRepository _pagamentos;

    public PagamentoService(IPedidoRepository pedidos, IPagamentoRepository pagamentos)
    {
        _pedidos = pedidos;
        _pagamentos = pagamentos;
    }

    // Devolve o pedido se ele puder receber um pagamento; senão lança DominioException
    public async Task<Pedido> ObterPedidoPagavelAsync(int pedidoId)
    {
        var pedido = await _pedidos.ObterPorIdAsync(pedidoId)
            ?? throw new DominioException("Pedido não encontrado.");

        if (pedido.Status == StatusPedido.Cancelado)
            throw new DominioException("Não é possível pagar um pedido cancelado.");

        if (pedido.Total <= 0)
            throw new DominioException("O pedido não possui itens para pagamento.");

        var existentes = await _pagamentos.ObterPorPedidoAsync(pedidoId);
        if (existentes.Any(p => p.Status == StatusPagamento.Aprovado))
            throw new DominioException("Este pedido já possui um pagamento aprovado.");

        return pedido;
    }
}
