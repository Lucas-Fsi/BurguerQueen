namespace BurguerQueen.Repositories.Interfaces;

//ciração da interface Genérica no padrão CRUD para qualquer entidade
//Usamos 'where t' para obrigar T sempre ser uma classe
//Garantindo as colçoes tipadas corretamente

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<T?> ObterPorIdAsync(int id);
    Task<int> AdicionarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task RemoverAsync(int id);
}