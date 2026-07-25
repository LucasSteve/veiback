namespace VeiCards.Aplicacao.Dtos;

public record CartaResponse(Guid Id, string Nome, string? Numero, string? Expansao, string? Raridade, string? Jogo, string? ImagemUrl);

public record CriarOuAtualizarCartaRequest(string Nome, string? Numero, string? Expansao, string? Raridade, string? Jogo, string? ImagemUrl);

public record StatusCartaResponse(Guid CartaId, bool Tem, bool Quero, bool Favorito);

public record AtualizarStatusCartaRequest(bool Tem, bool Quero, bool Favorito);
