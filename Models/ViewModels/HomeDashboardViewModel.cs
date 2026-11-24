namespace BurguerQueen.Models.ViewModels;

public class HomeDashboardViewModel
{
    public int PedidosAbertos { get; set; }
    public int PedidosEmPreparo { get; set; }
    public int PedidosProntos { get; set; }
    public int TotalProdutos { get; set; }
    public List<PedidoListaViewModel> UltimosPedidos { get; set; } = new();
}
