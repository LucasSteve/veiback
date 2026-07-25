namespace VeiCards.Aplicacao.Dtos;

public record CartaColecionadaResponse(
    Guid Id,
    string Jogo,
    string CartaExternaId,
    string Nome,
    string? Numero,
    string? Raridade,
    string? ImagemUrl,
    bool Tem,
    bool Quero,
    bool Favorito);

public record AtualizarStatusColecaoRequest(
    string Nome,
    string? Numero,
    string? Raridade,
    string? ImagemUrl,
    bool Tem,
    bool Quero,
    bool Favorito);

public record JogoComContagemResponse(string Jogo, int Quantidade);
