using MeuProjetoMVC.Exceptions;

namespace MeuProjetoMVC.Models;

public class ItemPedido 
{
    public int ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    protected ItemPedido() { } 

    public ItemPedido(int produtoId, string nomeProduto, decimal precoUnitario, int quantidade)
    {
        if (produtoId <= 0)
            throw new DominioException("Produto inválido.");
        if (string.IsNullOrWhiteSpace(nomeProduto))
            throw new DominioException("O nome do produto é obrigatório.");
        if (precoUnitario <= 0)
            throw new DominioException("O preço do item deve ser maior que zero.");
        if (quantidade <= 0)
            throw new DominioException("A quantidade deve ser maior que zero.");

        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        PrecoUnitario = precoUnitario; 
        Quantidade = quantidade;
    }

    public void AumentarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new DominioException("A quantidade deve ser maior que zero.");
        Quantidade += quantidade;
    }
}