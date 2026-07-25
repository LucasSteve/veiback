namespace VeiCards.Aplicacao.Dtos;

/// <summary>Envelope padrão de resposta para qualquer listagem paginada da API.</summary>
public record ResultadoPaginado<T>(IReadOnlyList<T> Itens, int PaginaAtual, int TamanhoPagina, int TotalItens)
{
    public int TotalPaginas => TamanhoPagina == 0 ? 0 : (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);
}
