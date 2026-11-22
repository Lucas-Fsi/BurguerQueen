using BurguerQueen.Models.Enums;
using BurguerQueen.Exceptions;
using System.Globalization;

namespace BurguerQueen.Models;

public abstract class Pagamento
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public int Id { get; private set; }
    public int PedidoId { get; private set; }
    public decimal Valor { get; protected set; }
    public DateTime DataHora { get; protected set; }
    public StatusPagamento Status { get; protected set; } = StatusPagamento.Pendente;

    protected Pagamento(int pedidoId, decimal valor)
    {
        if (pedidoId <= 0)
            throw new DominioException("O pagamento precisa estar vinculado a um pedido.");

        if (valor <= 0)
            throw new DominioException("O valor do pagamento deve ser maior que zero.");

        PedidoId = pedidoId;
        Valor = valor;
        DataHora = DateTime.Now;
    }

    public void Confirmar()
    {
        if (Status != StatusPagamento.Pendente)
            throw new DominioException("Só é possível confirmar um pagamento pendente.");

        Status = StatusPagamento.Aprovado;
    }

    public void Cancelar()
    {
        if (Status != StatusPagamento.Pendente)
            throw new DominioException("Só é possível cancelar um pagamento pendente.");

        Status = StatusPagamento.Cancelado;
    }

    public void Estornar()
    {
        if (Status != StatusPagamento.Aprovado)
            throw new DominioException("Só é possível estornar um pagamento aprovado.");

        Status = StatusPagamento.Estornado;
    }

    public abstract string Descrever();

    protected static string FormatarValor(decimal valor) => valor.ToString("C", PtBr);
}