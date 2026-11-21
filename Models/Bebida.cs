using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models;

public class Bebida : Produto
{
    public double Tamanho {get; private set; }
    //construtor da bebida:
    
    public Bebida(string nome, decimal preco, double tamano, int id = 0)
    :base(nome, preco, TipoProduto.Bebida, id)
    {
        this.DefinirTamanho(tamano);
    }
    
    public void DefinirTamanho(double tamanho)
    {
        if (tamanho <= 0)
            throw new DominioException("O tamanho da bebida deve ser maior que zero!");
        this.Tamanho = tamanho;
    }
    //overite do metodo abstrato
    public override string Detalhes()
    {
        return $"Bebida: - {this.Tamanho} ml";
    }
}