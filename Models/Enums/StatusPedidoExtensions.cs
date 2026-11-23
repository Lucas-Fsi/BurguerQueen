namespace BurguerQueen.Models.Enums;

public static class StatusPedidoExtensions
{
    public static string Texto(this StatusPedido s) => s switch
    {
        StatusPedido.Aberto => "Aberto",
        StatusPedido.EmPreparo => "Em preparo",
        StatusPedido.Pronto => "Pronto",
        StatusPedido.Entregue => "Entregue",
        StatusPedido.Cancelado => "Cancelado",
        _ => s.ToString()
    };

    public static string BadgeClass(this StatusPedido s) => s switch
    {
        StatusPedido.Aberto => "bg-primary",
        StatusPedido.EmPreparo => "bg-warning text-dark",
        StatusPedido.Pronto or StatusPedido.Entregue => "bg-success",
        StatusPedido.Cancelado => "bg-danger",
        _ => "bg-secondary"
    };
}