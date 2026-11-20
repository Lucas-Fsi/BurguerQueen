using BurguerQueen.Exceptions;

namespace BurguerQueen.Models;

public class Pix : Pagamento
{
    public string Chave { get; private set; }
    public string CodigoTransacao { get; private set; }

    public Pix(decimal valor, string chave, string codigoTransacao) : base(valor)
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