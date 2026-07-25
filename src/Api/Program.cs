using Microsoft.EntityFrameworkCore;
using Serilog;
using VeiCards.Api.Extensoes;
using VeiCards.Api.Middlewares;
using VeiCards.Infraestrutura;
using VeiCards.Infraestrutura.Persistencia;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((contexto, configuracaoLog) =>
    configuracaoLog.ReadFrom.Configuration(contexto.Configuration));

builder.Services.AddControllers();

builder.Services.AdicionarInfraestrutura(builder.Configuration);
builder.Services.AdicionarServicosDaAplicacao();
builder.Services.AdicionarSeguranca(builder.Configuration);
builder.Services.AdicionarVersionamentoDeApi();
builder.Services.AdicionarSwagger();
builder.Services.AdicionarCors(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("VeiCardsDb") ?? string.Empty;
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql");

var app = builder.Build();

// Aplica migrations pendentes e garante o usuário admin inicial — permite subir a API
// do zero (`dotnet run`) sem nenhum passo manual além de configurar a connection string.
using (var escopoInicializacao = app.Services.CreateScope())
{
    var contexto = escopoInicializacao.ServiceProvider.GetRequiredService<VeiCardsDbContext>();

    // O provider InMemory (usado pelos testes de integração) não suporta Migrate.
    if (contexto.Database.IsRelational())
    {
        await contexto.Database.MigrateAsync();
    }
    else
    {
        await contexto.Database.EnsureCreatedAsync();
    }
}

await SeedInicial.AplicarAsync(app.Services);

app.UseSerilogRequestLogging();
app.UseMiddleware<MiddlewareTratamentoDeExcecoes>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/swagger/v1/swagger.json", "VEI Cards API v1"));
}

app.UseHttpsRedirection();
app.UseCors("PoliticaPadrao");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Necessário como classe parcial pública para os testes de integração (WebApplicationFactory<Program>).
public partial class Program
{
}
