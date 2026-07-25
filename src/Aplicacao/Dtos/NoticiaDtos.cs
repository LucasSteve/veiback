namespace VeiCards.Aplicacao.Dtos;

public record NoticiaResponse(
    Guid Id,
    string Titulo,
    string? Resumo,
    string? Conteudo,
    string? Categoria,
    string? AutorNome,
    DateTime DataPublicacao,
    int? TempoLeituraMinutos,
    string? ImagemUrl);

public record CriarOuAtualizarNoticiaRequest(
    string Titulo,
    string? Resumo,
    string? Conteudo,
    string? Categoria,
    DateTime? DataPublicacao,
    int? TempoLeituraMinutos,
    string? ImagemUrl);
