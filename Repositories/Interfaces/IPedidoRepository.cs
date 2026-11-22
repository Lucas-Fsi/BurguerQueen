using BurguerQueen.Models;
using BurguerQueen.Models.Enums;
using BurguerQueen.Models.ViewModels;

namespace BurguerQueen.Repositories.Interfaces;

public interface IPedidoRepository
{
    Task<IEnumerable<PedidoListaViewModel>> ListarAsync(StatusPedido? status);
    Task<Pedido?> ObterPorIdAsync(int id);
    Task<int> CriarAsync(Pedido pedido);
    Task AtualizarAsync(Pedido pedido);   
}