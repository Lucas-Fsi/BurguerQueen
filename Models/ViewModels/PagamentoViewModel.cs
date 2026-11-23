using System.ComponentModel.DataAnnotations;
using BurguerQueen.Exceptions;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Models.ViewModels;

// Recebe os dados do formulário de pagamento.
// Só [Required] na forma de pagamento: os campos específicos de cada forma são opcionais aqui,
// e o construtor da entidade lança DominioException se faltar algum (mesmo padrão do ProdutoViewModel).
public class PagamentoViewModel
{
    public int PedidoId { get; set; }

    // Apenas para exibição. O valor real é sempre recalculado a partir do pedido.
    public decimal ValorTotal { get; set; }

    [Required(ErrorMessage = "Selecione a forma de pagamento!")]
    [Display(Name = "Forma de pagamento")]
    public FormaPagamento? Forma { get; set; }

    // Dinheiro
    [Display(Name = "Valor recebido (R$)")]
    public decimal? ValorRecebido { get; set; }

    // Pix
    [StringLength(100, ErrorMessage = "A chave Pix deve ter no máximo 100 caracteres!")]
    [Display(Name = "Chave Pix")]
    public string? Chave { get; set; }

    [StringLength(100, ErrorMessage = "O código da transação deve ter no máximo 100 caracteres!")]
    [Display(Name = "Código da transação")]
    public string? CodigoTransacao { get; set; }

    // Cartão
    [Display(Name = "Tipo do cartão")]
    public TipoCartao? TipoCartao { get; set; }

    [Display(Name = "Parcelas")]
    public int? Parcelas { get; set; } = 1;

    [StringLength(50, ErrorMessage = "A bandeira deve ter no máximo 50 caracteres!")]
    [Display(Name = "Bandeira")]
    public string? Bandeira { get; set; }

    [StringLength(50, ErrorMessage = "O código de autorização deve ter no máximo 50 caracteres!")]
    [Display(Name = "Código de autorização")]
    public string? CodigoAutorizacao { get; set; }

    // Converte o formulário na subclasse correta. O valor vem do pedido, nunca do formulário.
    public Pagamento ParaEntidade(int pedidoId, decimal valor)
    {
        return Forma switch
        {
            FormaPagamento.Dinheiro => new Dinheiro(
                pedidoId,
                valor,
                ValorRecebido ?? 0),

            FormaPagamento.Pix => new Pix(
                pedidoId,
                valor,
                Chave ?? string.Empty,
                CodigoTransacao ?? string.Empty),

            FormaPagamento.Cartao => new Cartao(
                pedidoId,
                valor,
                TipoCartao ?? default,
                Parcelas ?? 0,
                Bandeira ?? string.Empty,
                CodigoAutorizacao ?? string.Empty),

            _ => throw new DominioException("A forma de pagamento é inválida.")
        };
    }
}
