using MeuProjetoMVC.Exceptions;
using MeuProjetoMVC.Models.Enums;

namespace MeuProjetoMVC.Models;

public class Pedido 
{
    private readonly List<ItemPedido> _itens = new();
    public int? ClienteId { get; private set; }  
    public StatusPedido Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();
    public decimal Total => _itens.Sum(i => i.Subtotal);

    protected Pedido() { }

    public Pedido(int? clienteId)
    {
        ClienteId = clienteId;
        Status = StatusPedido.Aberto;
        DataCriacao = DateTime.Now;
    }

    public void CarregarItens(IEnumerable<ItemPedido> itens)
    {
        _itens.Clear();
        _itens.AddRange(itens);
    }

    public void AdicionarItem(int produtoId, string nomeProduto, decimal precoUnitario, int quantidade)
    {
        ExigirAberto();

        var existente = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (existente is not null)
        {
            existente.AumentarQuantidade(quantidade);
            return;
        }
        _itens.Add(new ItemPedido(produtoId, nomeProduto, precoUnitario, quantidade));
    }

    public void RemoverItem(int produtoId)
    {
        ExigirAberto();

        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId)
            ?? throw new DominioException("Item não encontrado no pedido.");
        _itens.Remove(item);
    }

    public void AvancarStatus()
    {
        switch (Status)
        {
            case StatusPedido.Aberto:
                if (_itens.Count == 0)
                    throw new DominioException("Adicione ao menos um item antes de enviar para o preparo.");
                Status = StatusPedido.EmPreparo;
                break;
            case StatusPedido.EmPreparo:
                Status = StatusPedido.Pronto;
                break;
            case StatusPedido.Pronto:
                Status = StatusPedido.Entregue;
                break;
            case StatusPedido.Entregue:
                throw new DominioException("O pedido já foi entregue.");
            case StatusPedido.Cancelado:
                throw new DominioException("Pedido cancelado não pode mudar de status.");
        }
    }

    public void Cancelar()
    {
        if (Status == StatusPedido.Entregue)
            throw new DominioException("Pedido entregue não pode ser cancelado.");
        if (Status == StatusPedido.Cancelado)
            throw new DominioException("O pedido já está cancelado.");
        Status = StatusPedido.Cancelado;
    }

    private void ExigirAberto()
    {
        if (Status != StatusPedido.Aberto)
            throw new DominioException("Só é possível alterar os itens de um pedido aberto.");
    }
}