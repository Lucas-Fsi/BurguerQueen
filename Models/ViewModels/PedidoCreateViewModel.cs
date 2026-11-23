using Microsoft.AspNetCore.Mvc.Rendering;

namespace BurguerQueen.Models.ViewModels;

public class PedidoCreateViewModel
{
    public int? ClienteId { get; set; }
    public List<SelectListItem> Clientes { get; set; } = new();
}