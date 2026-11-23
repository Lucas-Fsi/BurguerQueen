using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models.ViewModels;

public class PedidoListaViewModel
{
    public int Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public StatusPedido Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public decimal Total { get; set; }
}