using Microsoft.AspNetCore.Mvc.Rendering;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models.ViewModels;

public class PedidoDetalhesViewModel
{
    public int Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public StatusPedido Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public decimal Total { get; set; }
    public List<ItemPedidoViewModel> Itens { get; set; } = new();
    public List<SelectListItem> Produtos { get; set; } = new();

    public bool PodeEditarItens => Status == StatusPedido.Aberto;
    public bool PodeAvancar => Status is not (StatusPedido.Entregue or StatusPedido.Cancelado);
    public bool PodeCancelar => Status is not (StatusPedido.Entregue or StatusPedido.Cancelado);

    public string TextoAvancar => Status switch
    {
        StatusPedido.Aberto => "Enviar para o preparo",
        StatusPedido.EmPreparo => "Marcar como pronto",
        StatusPedido.Pronto => "Marcar como entregue",
        _ => string.Empty
    };
}