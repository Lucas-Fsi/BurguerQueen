using System.Diagnostics;
using BurguerQueen.Models;
using BurguerQueen.Models.Enums;
using BurguerQueen.Models.ViewModels;
using BurguerQueen.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BurguerQueen.Controllers;

public class HomeController : Controller
{
    private readonly IPedidoRepository _pedidos;
    private readonly IProdutoRepository _produtos;

    public HomeController(IPedidoRepository pedidos, IProdutoRepository produtos)
    {
        _pedidos = pedidos;
        _produtos = produtos;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeDashboardViewModel();

        try
        {
            var pedidos = (await _pedidos.ListarAsync(null)).ToList();
            var produtos = (await _produtos.ObterTodosAsync()).ToList();

            viewModel.PedidosAbertos = pedidos.Count(p => p.Status == StatusPedido.Aberto);
            viewModel.PedidosEmPreparo = pedidos.Count(p => p.Status == StatusPedido.EmPreparo);
            viewModel.PedidosProntos = pedidos.Count(p => p.Status == StatusPedido.Pronto);
            viewModel.TotalProdutos = produtos.Count;
            viewModel.UltimosPedidos = pedidos
                .OrderByDescending(p => p.DataCriacao)
                .ThenByDescending(p => p.Id)
                .Take(5)
                .ToList();
        }
        catch (Exception)
        {
            // Mantém o dashboard acessível mesmo se houver indisponibilidade temporária do banco
        }

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
