using System.Globalization;
using BurguerQueen.Exceptions;
using BurguerQueen.Models;
using BurguerQueen.Models.Enums;
using BurguerQueen.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BurguerQueen.Repositories.Dapper;

// Todas as formas de pagamento ficam na mesma tabela (Pagamentos).
// A coluna Forma indica qual subclasse (Dinheiro, Pix ou Cartao) deve ser reconstruída.
public class PagamentoRepository : IPagamentoRepository
{
    private const string Colunas =
        "Id, PedidoId, Forma, Valor, DataHora, Status, ValorRecebido, Chave, CodigoTransacao, TipoCartao, Parcelas, Bandeira, CodigoAutorizacao";

    private readonly string _connectionString;

    public PagamentoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada no appsettings.json");
    }

    private SqliteConnection CriarConexao() => new(_connectionString);

    public async Task<Pagamento?> ObterPorIdAsync(int id)
    {
        const string sql = $"SELECT {Colunas} FROM Pagamentos WHERE Id = @Id;";

        using var conexao = CriarConexao();
        var linha = await conexao.QueryFirstOrDefaultAsync<PagamentoRow>(sql, new { Id = id });
        return linha is null ? null : MapearParaEntidade(linha);
    }

    public async Task<IEnumerable<Pagamento>> ObterPorPedidoAsync(int pedidoId)
    {
        const string sql = $"SELECT {Colunas} FROM Pagamentos WHERE PedidoId = @PedidoId ORDER BY Id;";

        using var conexao = CriarConexao();
        var linhas = await conexao.QueryAsync<PagamentoRow>(sql, new { PedidoId = pedidoId });
        return linhas.Select(MapearParaEntidade).ToList();
    }

    public async Task<int> AdicionarAsync(Pagamento pagamento)
    {
        const string sql = """
            INSERT INTO Pagamentos (PedidoId, Forma, Valor, DataHora, Status, ValorRecebido, Chave, CodigoTransacao, TipoCartao, Parcelas, Bandeira, CodigoAutorizacao)
            VALUES (@PedidoId, @Forma, @Valor, @DataHora, @Status, @ValorRecebido, @Chave, @CodigoTransacao, @TipoCartao, @Parcelas, @Bandeira, @CodigoAutorizacao);
            SELECT last_insert_rowid();
            """;

        using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, CriarParametros(pagamento));
    }

    // Depois de criado, só o status de um pagamento muda
    public async Task AtualizarStatusAsync(Pagamento pagamento)
    {
        const string sql = "UPDATE Pagamentos SET Status = @Status WHERE Id = @Id;";

        using var conexao = CriarConexao();
        var linhasAfetadas = await conexao.ExecuteAsync(sql, new { Status = (int)pagamento.Status, pagamento.Id });

        if (linhasAfetadas == 0)
            throw new DominioException("Pagamento não encontrado para atualização.");
    }

    // Colunas que não pertencem à forma do pagamento vão como NULL
    private static object CriarParametros(Pagamento pagamento)
    {
        double? valorRecebido = null;
        string? chave = null;
        string? codigoTransacao = null;
        int? tipoCartao = null;
        int? parcelas = null;
        string? bandeira = null;
        string? codigoAutorizacao = null;

        switch (pagamento)
        {
            case Dinheiro dinheiro:
                valorRecebido = Convert.ToDouble(dinheiro.ValorRecebido);
                break;
            case Pix pix:
                chave = pix.Chave;
                codigoTransacao = pix.CodigoTransacao;
                break;
            case Cartao cartao:
                tipoCartao = (int)cartao.Tipo;
                parcelas = cartao.Parcelas;
                bandeira = cartao.Bandeira;
                codigoAutorizacao = cartao.CodigoAutorizacao;
                break;
        }

        return new
        {
            pagamento.PedidoId,
            Forma = (int)pagamento.Forma,
            Valor = Convert.ToDouble(pagamento.Valor),
            // Data gravada em texto ISO 8601 para evitar problemas de cultura na leitura
            DataHora = pagamento.DataHora.ToString("o"),
            Status = (int)pagamento.Status,
            ValorRecebido = valorRecebido,
            Chave = chave,
            CodigoTransacao = codigoTransacao,
            TipoCartao = tipoCartao,
            Parcelas = parcelas,
            Bandeira = bandeira,
            CodigoAutorizacao = codigoAutorizacao
        };
    }

    // Converte a linha do banco na subclasse correta, restaurando Id, data e status originais
    private static Pagamento MapearParaEntidade(PagamentoRow row)
    {
        var valor = Convert.ToDecimal(row.Valor);
        var dataHora = DateTime.Parse(row.DataHora, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        var status = (StatusPagamento)row.Status;

        return (FormaPagamento)row.Forma switch
        {
            FormaPagamento.Dinheiro => new Dinheiro(
                row.PedidoId, valor, Convert.ToDecimal(row.ValorRecebido ?? 0), row.Id, dataHora, status),

            FormaPagamento.Pix => new Pix(
                row.PedidoId, valor, row.Chave ?? string.Empty, row.CodigoTransacao ?? string.Empty,
                row.Id, dataHora, status),

            FormaPagamento.Cartao => new Cartao(
                row.PedidoId, valor, (TipoCartao)(row.TipoCartao ?? 0), row.Parcelas ?? 0,
                row.Bandeira ?? string.Empty, row.CodigoAutorizacao ?? string.Empty,
                row.Id, dataHora, status),

            _ => throw new DominioException($"Forma de pagamento inválida no banco de dados (ID: {row.Id}).")
        };
    }

    // Classe usada só para o Dapper receber as colunas do SELECT
    private sealed class PagamentoRow
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int Forma { get; set; }
        public double Valor { get; set; }
        public string DataHora { get; set; } = string.Empty;
        public int Status { get; set; }
        public double? ValorRecebido { get; set; }
        public string? Chave { get; set; }
        public string? CodigoTransacao { get; set; }
        public int? TipoCartao { get; set; }
        public int? Parcelas { get; set; }
        public string? Bandeira { get; set; }
        public string? CodigoAutorizacao { get; set; }
    }
}
