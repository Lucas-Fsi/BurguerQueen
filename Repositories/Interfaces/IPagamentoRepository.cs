using BurguerQueen.Models;

namespace BurguerQueen.Repositories.Interfaces;

// Pagamentos não são editados nem excluídos: só se registra e se muda o status
// (confirmar, cancelar, estornar). Por isso não herda de IRepository<T>.
public interface IPagamentoRepository
{
    Task<Pagamento?> ObterPorIdAsync(int id);
    Task<IEnumerable<Pagamento>> ObterPorPedidoAsync(int pedidoId);
    Task<int> AdicionarAsync(Pagamento pagamento);
    Task AtualizarStatusAsync(Pagamento pagamento);
}
