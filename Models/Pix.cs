using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models;

public class Pix : Pagamento
{
    public string Chave { get; private set; }
    public string CodigoTransacao { get; private set; } = string.Empty;

    public override FormaPagamento Forma => FormaPagamento.Pix;

    public Pix(
        int pedidoId,
        decimal valor,
        string chave,
        string codigoTransacao,
        int id = 0,
        DateTime? dataHora = null,
        StatusPagamento status = StatusPagamento.Pendente) : base(pedidoId, valor, id, dataHora, status)
    {
        Chave = chave;

        if (!ValidarChave())
            throw new DominioException("A chave Pix é obrigatória.");

        if (string.IsNullOrWhiteSpace(codigoTransacao))
            throw new DominioException("O código da transação é obrigatório.");

        CodigoTransacao = codigoTransacao;
    }

    public bool ValidarChave() => !string.IsNullOrWhiteSpace(Chave);

    public override string Descrever()
    => $"Pedido realizado no valor de {FormatarValor(Valor)} via Pix";
}
