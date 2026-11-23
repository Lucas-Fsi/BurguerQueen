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

    // Avança o status do pedido (Aberto → Em preparo → Pronto → Entregue).
    // Em qualquer etapa o pedido precisa ter um pagamento aprovado: se o pagamento
    // for estornado ou cancelado, o pedido fica parado até ser pago de novo.
    public async Task AvancarStatusAsync(int pedidoId)
    {
        var pedido = await _pedidos.ObterPorIdAsync(pedidoId)
            ?? throw new DominioException("Pedido não encontrado.");

        // Primeiro as regras do próprio pedido (itens, cancelado, entregue...),
        // assim a mensagem de pagamento não esconde um erro mais específico
        pedido.AvancarStatus();

        var pagamentos = await _pagamentos.ObterPorPedidoAsync(pedidoId);
        if (!pagamentos.Any(p => p.Status == StatusPagamento.Aprovado))
            throw new DominioException("O pedido precisa ter um pagamento aprovado para avançar.");

        await _pedidos.AtualizarAsync(pedido);
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
