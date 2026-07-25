using System.Net;
using Microsoft.AspNetCore.Mvc;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Api.Middlewares;

/// <summary>
/// Middleware global de tratamento de exceções. Traduz exceções de domínio para
/// respostas HTTP padronizadas (ProblemDetails) e garante que nenhum StackTrace
/// vaze para o cliente. Qualquer exceção não mapeada vira 500 genérico, logado via Serilog.
/// </summary>
public class MiddlewareTratamentoDeExcecoes
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<MiddlewareTratamentoDeExcecoes> _logger;

    public MiddlewareTratamentoDeExcecoes(RequestDelegate proximo, ILogger<MiddlewareTratamentoDeExcecoes> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (Exception excecao)
        {
            await TratarAsync(contexto, excecao);
        }
    }

    private async Task TratarAsync(HttpContext contexto, Exception excecao)
    {
        var (status, titulo) = excecao switch
        {
            ExcecaoDeEntidadeNaoEncontrada => (HttpStatusCode.NotFound, "Recurso não encontrado"),
            ExcecaoDeRegraDeNegocio => (HttpStatusCode.UnprocessableEntity, "Regra de negócio violada"),
            ArgumentException => (HttpStatusCode.BadRequest, "Requisição inválida"),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor"),
        };

        if (status == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(excecao, "Erro não tratado ao processar {Metodo} {Caminho}", contexto.Request.Method, contexto.Request.Path);
        }
        else
        {
            _logger.LogWarning(excecao, "Erro de negócio ao processar {Metodo} {Caminho}: {Mensagem}", contexto.Request.Method, contexto.Request.Path, excecao.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int)status,
            Title = titulo,
            Detail = status == HttpStatusCode.InternalServerError ? "Ocorreu um erro inesperado. Tente novamente mais tarde." : excecao.Message,
            Instance = contexto.Request.Path,
        };

        contexto.Response.ContentType = "application/problem+json";
        contexto.Response.StatusCode = (int)status;
        await contexto.Response.WriteAsJsonAsync(problemDetails);
    }
}
