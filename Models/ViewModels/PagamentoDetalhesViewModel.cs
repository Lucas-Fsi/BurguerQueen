using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models.ViewModels;

public class PagamentoDetalhesViewModel
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataHora { get; set; }
    public StatusPagamento Status { get; set; }

    public bool PodeConfirmar => Status == StatusPagamento.Pendente;
    public bool PodeCancelar => Status == StatusPagamento.Pendente;
    public bool PodeEstornar => Status == StatusPagamento.Aprovado;

    public static PagamentoDetalhesViewModel DeEntidade(Pagamento pagamento) => new()
    {
        Id = pagamento.Id,
        PedidoId = pagamento.PedidoId,
        Descricao = pagamento.Descrever(),
        Valor = pagamento.Valor,
        DataHora = pagamento.DataHora,
        Status = pagamento.Status
    };
}
