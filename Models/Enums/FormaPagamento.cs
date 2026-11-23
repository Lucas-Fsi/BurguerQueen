using System.ComponentModel.DataAnnotations;

namespace BurguerQueen.Models.Enums;

// Os valores são gravados no banco (coluna Forma) e definem qual subclasse de Pagamento criar
public enum FormaPagamento
{
    Dinheiro = 1,
    Pix = 2,

    [Display(Name = "Cartão")]
    Cartao = 3
}
