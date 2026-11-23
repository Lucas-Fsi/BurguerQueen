using System.ComponentModel.DataAnnotations;
using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models.ViewModels;

// Viewmodel será usado para transportar dados etre o controller e as views
//QUando o usuario envia um formulario. o Viwmodel recebe os dados
//e os converte para a entidade corrita (bebida ou lanche)

public class ProdutoViewModel
{

    public int Id { get; set; }

    //Data Annotations para validar os dados
    //e definir mensagens de erro com rotulos amigaveis

    // NOME DO PRODUTO
    [Required(ErrorMessage = "O nome do produto é obrigatório!")]
    [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres!")]
    [Display(Name = "Nome do Produto")]
    public string Nome { get; set; } = string.Empty;

    //PREÇO DO PRODUTO
    [Required (ErrorMessage = "O preço do produto é obrigatório!")]
    [Range(0.01, 9999.99, ErrorMessage = "O preço do produto deve estar entre 0,01 e 9999,99!")]
    [Display(Name = "Preço (R$)")]
    public decimal Preco { get; set; }

    //TIPO DO PRODUTO
    [Required(ErrorMessage = "Selecione o tipo do produto!")]
    [Display(Name = "Tipo do Produto")]
    public TipoProduto Tipo { get; set; }

    //
    //CAMPOS E OPVIONAIS, ESPECIFICOS Ficam com ?
    // pois nem todos os produtos terão esses campos

    //DESCRIÇÃO DO PRODUTO
    [StringLength(250, ErrorMessage = "A descrição do lanche deve ter no máximo 250 caracteres!")]
    [Display(Name = "Descrição do Lanche")]
    public string? Descricao { get; set; }

    //ADICIONAIS
    [StringLength(150, ErrorMessage = "Os Adicionais devem ter no máximo 150 caracteries!")]
    [Display(Name = "Adicionais")]
    public string? Adicionais { get ; set; }
    
    //TAMANHO DA BEBIDA
    [Range(1, 5000, ErrorMessage = "O tamanho da bebida deve estar entre 1 e 5000 (em ml)!")]
    [Display(Name = "Tamanho (ml)")]
    public double? Tamanho { get; set; }

    //Converte dos dados dos formularios para endidades corretas
    //Caso de campo vazio, o construtor ira disparar uma DomimioException
    // para garantir que a refra de negocio seja mantida

    public Produto ParaEntidade()
    {
        return this.Tipo switch
        {
        
        TipoProduto.Lanche => new Lanche(
            this.Nome,
            this.Preco,
            this.Descricao ?? string.Empty,
            this.Adicionais,
            this.Id),
    
        TipoProduto.Bebida => new Bebida(
            this.Nome,
            this.Preco,
            this.Tamanho ?? 0,
            this.Id),

            _ => throw new DominioException("O Tipo de Produto é invalido")
        };
    }

    //METODO AUXILIAR estático para fazer o caminho inverso (Editar)
    //recebe a entidade via Dapper e preenche o viewmodel

    public static ProdutoViewModel DeEntidade(Produto produto)
    {
        var viewModel = new ProdutoViewModel
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            Tipo = produto.Tipo
        };

        // "Is" verifica se a classe é filha 
        // foi usado para copiar os atributos especificos de Lanche/bebida

        if (produto is Lanche lanche)
        {
            viewModel.Descricao = lanche.Descricao;
            viewModel.Adicionais = lanche.Adicionais;
        }
        else if (produto is Bebida bebida)
        {
            viewModel.Tamanho = bebida.Tamanho;
        }
        return viewModel;
    }
}