using BurguerQueen.Exceptions;
using BurguerQueen.Models;
using BurguerQueen.Models.Enums;
using BurguerQueen.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BurguerQueen.Repositories.Dapper;

//implementa o repositorio padrão usando o Dapper
//o acesso do banco e codigo sql fica isolado aqui

public class ProdutoRepository : IProdutoRepository
{
    private readonly string _connectionString;

    //A confg do appsettings.json cheea via injeção de dependencia

    public ProdutoRepository(IConfiguration configuration)
    {
        this._connectionString = configuration.GetConnectionString("DefaultConnection")
            ??throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada no appsettings.json");
    }

    //Cria a conexão ADO.NET para SQLite

    private SqliteConnection CriarConexao()
        => new SqliteConnection(this._connectionString);

    public async Task<IEnumerable<Produto>> ObterTodosAsync()
    {
        const string sql = """
            SELECT Id, Nome, Preco, Tipo, Descricao, Adicionais, Tamanho
            FROM Produtos
            ORDER BY Tipo, Nome;
            """;
        
        //Garantir será fechada e liberada da memoria quando finalizar
        using var conexao = this.CriarConexao();

        //o Dapper vai executar o select (do sql) e retornar para o objeto temporario ProdutoRow
        var linhas = await conexao.QueryAsync<ProdutoRow>(sql);

        //vai converter as linhas do banco para entidade correta
        return linhas.Select(MapearParaEntidade).ToList();
    }
    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        //quary com parametro '@Id' para evitar junção0 de string
        //isso tabm protege contra sql injection

        const string sql = """
        SELECT Id, Nome, Preco, Tipo, Descricao, Adicionais, Tamanho
        FROM Produtos
        WHERE Id = @Id;
        """;

        using var conexao = this.CriarConexao();

        //parametros seram passados como objeto anônimo para o dapper
        var linha = await conexao.QueryFirstOrDefaultAsync<ProdutoRow>(sql, new {Id = id});
        return linha is null ? null : MapearParaEntidade(linha);
    }
    public async Task<IEnumerable<Produto>> ObterPorTipoAsync(TipoProduto tipo)
    {
        const string sql = """
        SELECT Id, Nome, Preco, Tipo, Descricao, Adicionais, Tamanho
        FROM Produtos
        WHERE Tipo = @Tipo
        ORDER BY Nome;
        """;

        using var conecao = this.CriarConexao();
        var linhas = await conecao.QueryAsync<ProdutoRow>(sql, new { Tipo = (int)tipo });
        
        return linhas.Select(MapearParaEntidade).ToList();
    }

    public async Task<int> AdicionarAsync(Produto entidade)
    {
        // last_insert_rowid retorna o id gerado Automaticamente 
        const string sql = """
        INSERT INTO Produtos (Nome, Preco, Tipo, Descricao, Adicionais, Tamanho)
        VALUES (@Nome, @Preco, @Tipo, @Descricao, @Adicionais, @Tamanho);
        SELECT last_insert_rowid()
        """;

        using var conexao = this.CriarConexao();

        //extrai os paramestos
        var parametros = CriarParametros(entidade);
        return await conexao.ExecuteScalarAsync<int>(sql, parametros);
    }

    public async Task AtualizarAsync(Produto entidade)
    {
        const string sql = """
            UPDATE Produtos
            SET Nome = @Nome,
                Preco = @Preco,
                Tipo = @Tipo,
                Descricao = @Descricao,
                Adicionais = @Adicionais,
                Tamanho = @Tamanho
            WHERE Id = @Id;
        """;
        using var conexao = this.CriarConexao();
        var parametros = CriarParametros(entidade);
        var linhasAfetadas = await conexao.ExecuteAsync(sql, parametros);
        if (linhasAfetadas == 0)
            throw new DominioException("Produto não Encontrado para ataualização.");
    }

    public async Task RemoverAsync(int id)
    {
        const string sql = "DELETE FROM Produtos WHERE Id = @Id;";

        using var conexao = this.CriarConexao();
        var linhasAfetadas = await conexao.ExecuteAsync(sql, new {Id = id});

        if (linhasAfetadas == 0)
        throw new DominioException("Produto não encontrado para exclusão");
    }

    //monta o objeto anonimo com os parametros sql
    //Compo que nãopertemcem a subclasse vão como null para o sql

    private static object CriarParametros(Produto entidade)
    {
        string? descricao = null;
        string? adicionais = null;
        double? tamanho = null;

        if (entidade is Lanche lanche)
        {
            descricao = lanche.Descricao;
            adicionais = lanche.Adicionais;
        }
        else if (entidade is Bebida bebida)
        {
            tamanho = bebida.Tamanho;
        }

        return new
        {
            entidade.Id,
            entidade.Nome,
            entidade.Preco,
            Tipo = (int)entidade.Tipo,
            Descricao = descricao,
            Adicionais = adicionais,
            Tamanho = tamanho
        };
    }
    //Conveter a linha do dapper em uma entidade do dominio
    private static Produto MapearParaEntidade(ProdutoRow row)
    {
        var tipo = (TipoProduto)row.Tipo;
        var preco = Convert.ToDecimal(row.Preco);

        return tipo switch
        {
            TipoProduto.Lanche => new Lanche(
                row.Nome,
                preco,
                row.Descricao ?? string.Empty,
                row.Adicionais,
                row.Id),
            TipoProduto.Bebida => new Bebida(
                row.Nome,
                preco,
                row.Tamanho ?? 0,
                row.Id),
            _ => throw new DominioException($"Tipo de produto inválido no Banco de dados (ID: {row.Id}.)")
        };
    }
    // Classe privada isada para o Dapper receber as Colunas do Select

    private sealed class ProdutoRow
    {
        public int Id{ get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Preco { get; set; }
        public int Tipo {get; set; }
        public string? Descricao { get; set; }
        public string? Adicionais { get; set; }
        public double? Tamanho { get; set; }
    }
}