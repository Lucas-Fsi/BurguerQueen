using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;
using BurguerQueen.Repositories.Interfaces;

namespace BurguerQueen.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IPagamentoRepository _pagamentos;

    public PedidoService(IPedidoRepository pedidos, IPagamentoRepository pagamentos)
    {
        _pedidos = pedidos;
        _pagamentos = pagamentos;
    }

    // Cancela o pedido e acerta os pagamentos dele:
    // pagamento aprovado é estornado e pagamento pendente é cancelado.
    public async Task CancelarAsync(int pedidoId)
    {
        var pedido = await _pedidos.ObterPorIdAsync(pedidoId)
            ?? throw new DominioException("Pedido não encontrado.");

        // Valida primeiro: se o pedido não puder ser cancelado, nenhum pagamento é alterado
        pedido.Cancelar();

        var pagamentos = await _pagamentos.ObterPorPedidoAsync(pedidoId);
        foreach (var pagamento in pagamentos)
        {
            if (pagamento.Status == StatusPagamento.Aprovado)
                pagamento.Estornar();
            else if (pagamento.Status == StatusPagamento.Pendente)
                pagamento.Cancelar();
            else
                continue;

            await _pagamentos.AtualizarStatusAsync(pagamento);
        }

        await _pedidos.AtualizarAsync(pedido);
    }
}
