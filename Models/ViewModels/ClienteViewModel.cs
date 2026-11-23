using System.ComponentModel.DataAnnotations;

namespace BurguerQueen.Models.ViewModels
{
    public class ClienteViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string CPF { get; set; } = string.Empty;
    }
}