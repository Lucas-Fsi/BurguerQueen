namespace BurguerQueen.Models.Enums;

public static class StatusPagamentoExtensions
{
    public static string Texto(this StatusPagamento s) => s switch
    {
        StatusPagamento.Pendente => "Pendente",
        StatusPagamento.Aprovado => "Aprovado",
        StatusPagamento.Cancelado => "Cancelado",
        StatusPagamento.Estornado => "Estornado",
        _ => s.ToString()
    };

    public static string BadgeClass(this StatusPagamento s) => s switch
    {
        StatusPagamento.Pendente => "bg-warning text-dark",
        StatusPagamento.Aprovado => "bg-success",
        StatusPagamento.Cancelado => "bg-secondary",
        StatusPagamento.Estornado => "bg-danger",
        _ => "bg-secondary"
    };
}
