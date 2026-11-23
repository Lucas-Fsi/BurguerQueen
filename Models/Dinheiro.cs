using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models;

public class Dinheiro : Pagamento
{
    public decimal ValorRecebido { get; private set; }

    public override FormaPagamento Forma => FormaPagamento.Dinheiro;

    public Dinheiro(
        int pedidoId,
        decimal valor,
        decimal valorRecebido,
        int id = 0,
        DateTime? dataHora = null,
        StatusPagamento status = StatusPagamento.Pendente) : base(pedidoId, valor, id, dataHora, status)
    {
        ValorRecebido = valorRecebido;

        if (!ValidarValorRecebido())
            throw new DominioException("O valor recebido não pode ser menor que o valor do pagamento.");
    }

    public bool ValidarValorRecebido() => ValorRecebido >= Valor;

    public decimal CalcularTroco() => ValorRecebido - Valor;

    public override string Descrever()
    {
        var troco = CalcularTroco();
        var descricao = $"Pedido realizado no valor de {FormatarValor(Valor)} em dinheiro";

        return troco > 0
            ? $"{descricao}, troco de {FormatarValor(troco)}"
            : $"{descricao}, sem troco";
    }
}
