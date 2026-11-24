using BurguerQueen.Models;

namespace BurguerQueen.Repositories.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> ObterPorCpfAsync(string cpf);
}