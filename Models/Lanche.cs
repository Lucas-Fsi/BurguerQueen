using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models;

public class Lanche : Produto
{
        //privete set para proteger os atrubutos específicos do lanche,
        // para que não sejam alterados por um código externo
            public string Descricao { get; private set; } = string.Empty;
            public string? Adicionais { get; private set; }
        // ? para indicar que pode ser nulo

        //Construtor de lanche, já fixando o tipo como lanche.
        public Lanche(string nome, decimal preco, string descricao, string? adicionais = null, int id = 0)
        :base (nome, preco, TipoProduto.Lanche, id)
        {
            this.DefinirDescricao(descricao);
            this.DefinirAdicionais(adicionais);
        }

        //metodos de dominio 
        public void DefinirDescricao(string descticao)
    {
        if (string.IsNullOrWhiteSpace(descticao))
            throw new DominioException("A descrição do lanche é obrigatória!");

        this.Descricao = descticao.Trim();
    }
    public void DefinirAdicionais(string? adicionais)
    {
        this.Adicionais = string.IsNullOrWhiteSpace(adicionais)
        ? null 
        : adicionais.Trim();
    }
            
    //sobrescreve o metodo abstrato, para fornecer detalhes especificos do lanche
        public override string Detalhes()
    {
        if (string.IsNullOrWhiteSpace(this.Adicionais))
            return $"Lanche: {this.Descricao}";

        return $"Lanche: {this.Descricao} | Adicionais: {this.Adicionais}";
    }
}
