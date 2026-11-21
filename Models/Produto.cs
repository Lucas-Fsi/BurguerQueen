using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models;

//classe modelo (abstrata) base para Lanche e bebidas
public abstract class Produto
{
    
    // usamos protected set para que podemos setar
    // o nome do produto apenas dentro da classe Produto e suas classes filhas (Lanche e Bebida)
    public int Id { get; protected set; }
    
    //e o privete set para impedir impedir alteração por um código externo,
    // fazendo com que o objeto nao fique com estado inválido
    public string Nome {get; private set;} = string.Empty;
    public decimal Preco {get; private set; }
    public TipoProduto Tipo {get; protected set;}

    protected Produto(string nome, decimal preco, TipoProduto tipo, int id = 0)
    {
        if (id < 0)
            throw new DominioException("O Id do produto não pode ser negativo");
        if (!Enum.IsDefined(typeof(TipoProduto), tipo))
            throw new DominioException("O Tipo do produto informado é inválido");

        this.Id = id; 
        this.Tipo = tipo;

        //metodos de dominio para validar e definir o nome e preco do produto
        //usado aqui para centralizar a validação aqui
        DefinirNome(nome);
        DefinirPreco(preco);
    }
    //metodos do dominio para validar e definir o nome e preco do produto
    //conforme as regras de negócio
    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DominioException("O nome do produto é obrigatório!");
            //trim para recortar espaços em branco no inicio e fim do nome do produto
            this.Nome = nome.Trim();
    }
    public void DefinirPreco(decimal preco)
    {
        if (preco < 0)
            throw new DominioException("O preço do produto deve ser Maior que Zero!");
        this.Preco = preco;
    }
    //metodo abstrato que obriga as classes filhas(lanche e bebida) a implementarem o metodo Detalhes
    public abstract string Detalhes();
}
