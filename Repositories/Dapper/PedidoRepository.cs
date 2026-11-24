using Dapper;
using Microsoft.Data.Sqlite;
using BurguerQueen.Models;
using BurguerQueen.Models.Enums;
using BurguerQueen.Models.ViewModels;
using BurguerQueen.Repositories.Interfaces;

namespace BurguerQueen.Repositories.Dapper;

public class PedidoRepository : IPedidoRepository
{
    private readonly string _connectionString;

    public PedidoRepository(IConfiguration config)
        => _connectionString = config.GetConnectionString("DefaultConnection")!;

 public async Task<IEnumerable<PedidoListaViewModel>> ListarAsync(StatusPedido? status)
{
    const string sql = @"
        SELECT p.Id,
               COALESCE(c.Nome, 'Balcão') AS NomeCliente,
               p.Status,
               p.DataCriacao,
               COALESCE(SUM(i.Quantidade * i.PrecoUnitario), 0) AS Total
        FROM Pedidos p
        LEFT JOIN Clientes c ON c.Id = p.ClienteId
        LEFT JOIN ItensPedido i ON i.PedidoId = p.Id
        WHERE (@Status IS NULL OR p.Status = @Status)
        GROUP BY p.Id, c.Nome, p.Status, p.DataCriacao
        ORDER BY p.Id DESC";

    using var conn = new SqliteConnection(_connectionString);
    return await conn.QueryAsync<PedidoListaViewModel>(sql, new { Status = (int?)status });
}

    public async Task<Pedido?> ObterPorIdAsync(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var pedido = await conn.QueryFirstOrDefaultAsync<Pedido>(
            "SELECT Id, ClienteId, Status, DataCriacao FROM Pedidos WHERE Id = @Id",
            new { Id = id });

        if (pedido is null) return null;

        var itens = await conn.QueryAsync<ItemPedido>(
            @"SELECT Id, ProdutoId, NomeProduto, Quantidade, PrecoUnitario
              FROM ItensPedido WHERE PedidoId = @Id",
            new { Id = id });

        pedido.CarregarItens(itens);
        return pedido;
    }

    public async Task<int> CriarAsync(Pedido pedido)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO Pedidos (ClienteId, Status, DataCriacao)
              VALUES (@ClienteId, @Status, @DataCriacao);
              SELECT last_insert_rowid();",
            new { pedido.ClienteId, Status = (int)pedido.Status, pedido.DataCriacao });
    }

    public async Task AtualizarAsync(Pedido pedido)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        await using var tx = await conn.BeginTransactionAsync();

        await conn.ExecuteAsync(
            "UPDATE Pedidos SET Status = @Status WHERE Id = @Id",
            new { Status = (int)pedido.Status, pedido.Id }, tx);

        await conn.ExecuteAsync(
            "DELETE FROM ItensPedido WHERE PedidoId = @Id",
            new { pedido.Id }, tx);

        foreach (var item in pedido.Itens)
        {
            await conn.ExecuteAsync(
                @"INSERT INTO ItensPedido (PedidoId, ProdutoId, NomeProduto, Quantidade, PrecoUnitario)
                  VALUES (@PedidoId, @ProdutoId, @NomeProduto, @Quantidade, @PrecoUnitario)",
                new
                {
                    PedidoId = pedido.Id,
                    item.ProdutoId,
                    item.NomeProduto,
                    item.Quantidade,
                    item.PrecoUnitario
                }, tx);
        }

        await tx.CommitAsync();
    }
}