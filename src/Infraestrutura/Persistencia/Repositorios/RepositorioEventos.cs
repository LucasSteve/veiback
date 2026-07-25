using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioEventos : IRepositorioEventos
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioEventos(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<Evento?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        _contexto.Eventos.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<(IReadOnlyList<Evento> Itens, int Total)> ListarAsync(FiltroEventos filtro, CancellationToken ct = default)
    {
        var consulta = _contexto.Eventos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Cidade))
        {
            consulta = consulta.Where(e => e.Cidade == filtro.Cidade);
        }

        if (filtro.Tipo is { } tipo)
        {
            consulta = consulta.Where(e => e.Tipo == tipo);
        }

        // Status é calculado a partir da Data (ver Evento.CalcularStatus), então o filtro
        // por status é traduzido aqui para uma condição de data equivalente.
        if (filtro.Status is { } status)
        {
            var hoje = DateTime.UtcNow.Date;
            consulta = status switch
            {
                StatusEvento.Encerrado => consulta.Where(e => e.Data.Date < hoje),
                StatusEvento.AoVivo => consulta.Where(e => e.Data.Date == hoje),
                StatusEvento.EmBreve => consulta.Where(e => e.Data.Date > hoje),
                _ => consulta,
            };
        }

        consulta = consulta.OrderBy(e => e.Data);

        var total = await consulta.CountAsync(ct);
        var itens = await consulta.Skip((filtro.Pagina - 1) * filtro.TamanhoPagina).Take(filtro.TamanhoPagina).ToListAsync(ct);
        return (itens, total);
    }

    public Task<int> ContarAsync(CancellationToken ct = default) => _contexto.Eventos.CountAsync(ct);

    public async Task AdicionarAsync(Evento evento, CancellationToken ct = default)
    {
        await _contexto.Eventos.AddAsync(evento, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Evento evento, CancellationToken ct = default)
    {
        _contexto.Eventos.Update(evento);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Evento evento, CancellationToken ct = default)
    {
        _contexto.Eventos.Remove(evento);
        await _contexto.SaveChangesAsync(ct);
    }
}
