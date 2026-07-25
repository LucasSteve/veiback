namespace VeiCards.Aplicacao.Dtos;

public record EventoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    DateTime Data,
    string? Horario,
    string? Local,
    string? Cidade,
    string? Organizador,
    string? Formato,
    string Tipo,
    string Status,
    int? Capacidade,
    int VagasOcupadas,
    string? ImagemUrl,
    bool InscricoesAbertas);

public record CriarOuAtualizarEventoRequest(
    string Nome,
    string? Descricao,
    DateTime Data,
    string? Horario,
    string? Local,
    string? Cidade,
    string? Organizador,
    string? Formato,
    string Tipo,
    int? Capacidade,
    string? ImagemUrl);

public record AtualizarInscricoesAbertasRequest(bool Abertas);

public record InscricaoEventoResponse(Guid EventoId, Guid UsuarioId, DateTime DataInscricao);
