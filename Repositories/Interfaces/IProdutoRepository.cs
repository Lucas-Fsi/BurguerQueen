using BurguerQueen.Models;
using BurguerQueen.Models.Enums;

namespace BurguerQueen.Repositories.Interfaces;

//interface especificas dos prodtos 
//herda de repositorio e adiciona a consulta de dominio de produto

public interface IProdutoRepository : IRepository<Produto>
{
    //task que permite filtrar o cardapio por lanche ou bebidas 
    Task<IEnumerable<Produto>> ObterPorTipoAsync(TipoProduto tipo);
}