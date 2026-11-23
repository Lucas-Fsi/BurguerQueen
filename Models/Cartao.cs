using BurguerQueen.Exceptions;

namespace BurguerQueen.Models;

public class Cartao : Pagamento
{
    private const int MaxParcelas = 12;

    public int Parcelas { get; private set; }
    public string Bandeira { get; private set; }
    public string CodigoAutorizacao { get; private set; }

    public Cartao(int pedidoId, decimal valor, int parcelas, string bandeira, string codigoAutorizacao) : base(pedidoId, valor)
    {
        Parcelas = parcelas;

        if (!ValidarParcelas())
            throw new DominioException($"O número de parcelas deve estar entre 1 e {MaxParcelas}.");

        if (string.IsNullOrWhiteSpace(bandeira))
            throw new DominioException("A bandeira do cartão é obrigatória.");

        if (string.IsNullOrWhiteSpace(codigoAutorizacao))
            throw new DominioException("O código de autorização é obrigatório.");

        Bandeira = bandeira;
        CodigoAutorizacao = codigoAutorizacao;
    }

    public bool ValidarParcelas() => Parcelas >= 1 && Parcelas <= MaxParcelas;

    public decimal CalcularValorParcela() => Math.Round(Valor / Parcelas, 2);

    public override string Descrever()
        => $"Pedido realizado no valor de {FormatarValor(Valor)} no cartão ({Bandeira}) em {Parcelas}x de {FormatarValor(CalcularValorParcela())}";
}