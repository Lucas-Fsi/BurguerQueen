using BurguerQueen.Repositories.Dapper;
using BurguerQueen.Repositories.Interfaces;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//injeção de dependencias
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
var app = builder.Build();

using (var conn = new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    conn.Open();

    using var check = conn.CreateCommand();
    check.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Produtos'";
    var existe = (long)check.ExecuteScalar()! > 0;

    if (!existe)
    {
        var caminho = Path.Combine(builder.Environment.ContentRootPath, "Database", "script.sql");
        using var cmd = conn.CreateCommand();
        cmd.CommandText = File.ReadAllText(caminho);
        cmd.ExecuteNonQuery();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
