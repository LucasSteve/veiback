using Serilog;
using VeiCards.Api.Extensoes;
using VeiCards.Api.Middlewares;
using VeiCards.Infraestrutura;

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
