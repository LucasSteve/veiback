namespace VeiCards.Aplicacao.Filtros;

public record FiltroCartas(string? Busca, string? Jogo, string? Raridade, string? OrdenarPor, int Pagina, int TamanhoPagina);
