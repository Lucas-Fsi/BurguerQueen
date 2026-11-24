using System.Data;
using BurguerQueen.Models;
using BurguerQueen.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BurguerQueen.Repositories.Dapper
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;

        public ClienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não configurada.");
        }

        private IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            const string sql = "SELECT Id, Nome, Telefone, CPF FROM Clientes ORDER BY Nome;";
            using var connection = CreateConnection();
            return await connection.QueryAsync<Cliente>(sql);
        }

        public async Task<Cliente?> ObterPorIdAsync(int id)
        {
            const string sql = "SELECT Id, Nome, Telefone, CPF FROM Clientes WHERE Id = @Id;";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
        }

        public async Task<Cliente?> ObterPorCpfAsync(string cpf)
        {
            const string sql = "SELECT Id, Nome, Telefone, CPF FROM Clientes WHERE CPF = @CPF;";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { CPF = cpf });
        }

        public async Task<int> AdicionarAsync(Cliente entidade)
        {
            const string sql = @"
                INSERT INTO Clientes (Nome, Telefone, CPF)
                VALUES (@Nome, @Telefone, @CPF);
                SELECT last_insert_rowid();";

            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, entidade);
        }

        public async Task AtualizarAsync(Cliente entidade)
        {
            const string sql = @"
                UPDATE Clientes
                SET Nome = @Nome,
                    Telefone = @Telefone,
                    CPF = @CPF
                WHERE Id = @Id;";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, entidade);
        }

        public async Task RemoverAsync(int id)
        {
            const string sql = "DELETE FROM Clientes WHERE Id = @Id;";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}