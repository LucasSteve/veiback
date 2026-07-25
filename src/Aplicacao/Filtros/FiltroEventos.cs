using VeiCards.Dominio.Enums;

namespace VeiCards.Aplicacao.Filtros;

public record FiltroEventos(string? Cidade, TipoEvento? Tipo, StatusEvento? Status, int Pagina, int TamanhoPagina);
