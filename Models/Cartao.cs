using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models;

public class Cartao : Pagamento
{
    private const int MaxParcelas = 12;

    public TipoCartao Tipo { get; private set; }
    public int Parcelas { get; private set; }
    public string Bandeira { get; private set; } = string.Empty;
    public string CodigoAutorizacao { get; private set; } = string.Empty;

    public override FormaPagamento Forma => FormaPagamento.Cartao;

    public Cartao(
        int pedidoId,
        decimal valor,
        TipoCartao tipo,
        int parcelas,
        string bandeira,
        string codigoAutorizacao,
        int id = 0,
        DateTime? dataHora = null,
        StatusPagamento status = StatusPagamento.Pendente) : base(pedidoId, valor, id, dataHora, status)
    {
        if (!Enum.IsDefined(tipo))
            throw new DominioException("Selecione se o cartão é de débito ou crédito.");

        Tipo = tipo;
        Parcelas = parcelas;

        if (!ValidarParcelas())
            throw new DominioException(Tipo == TipoCartao.Debito
                ? "No cartão de débito o pagamento deve ser em 1 parcela."
                : $"O número de parcelas deve estar entre 1 e {MaxParcelas}.");

        if (string.IsNullOrWhiteSpace(bandeira))
            throw new DominioException("A bandeira do cartão é obrigatória.");

        if (string.IsNullOrWhiteSpace(codigoAutorizacao))
            throw new DominioException("O código de autorização é obrigatório.");

        Bandeira = bandeira;
        CodigoAutorizacao = codigoAutorizacao;
    }

    // Débito só aceita 1 parcela; crédito aceita de 1 até MaxParcelas
    public bool ValidarParcelas() => Tipo == TipoCartao.Debito
        ? Parcelas == 1
        : Parcelas >= 1 && Parcelas <= MaxParcelas;

    public decimal CalcularValorParcela() => Math.Round(Valor / Parcelas, 2);

    public override string Descrever()
    {
        var tipo = Tipo == TipoCartao.Debito ? "débito" : "crédito";
        var descricao = $"Pedido realizado no valor de {FormatarValor(Valor)} no cartão de {tipo} ({Bandeira})";

        return Tipo == TipoCartao.Credito
            ? $"{descricao} em {Parcelas}x de {FormatarValor(CalcularValorParcela())}"
            : descricao;
    }
}
