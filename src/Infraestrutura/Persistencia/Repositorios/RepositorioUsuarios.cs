using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioUsuarios(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<Usuario?> ObterPorNomeUsuarioAsync(string nomeUsuario, CancellationToken ct = default) =>
        _contexto.Usuarios.FirstOrDefaultAsync(u => u.NomeUsuario == nomeUsuario, ct);

    public Task<bool> ExisteComNomeUsuarioOuEmailAsync(string nomeUsuario, string email, CancellationToken ct = default) =>
        _contexto.Usuarios.AnyAsync(u => u.NomeUsuario == nomeUsuario || u.Email == email.ToLower(), ct);

    public async Task<(IReadOnlyList<Usuario> Itens, int Total)> ListarAsync(int pagina, int tamanhoPagina, CancellationToken ct = default)
    {
        var consulta = _contexto.Usuarios.OrderBy(u => u.NomeUsuario);
        var total = await consulta.CountAsync(ct);
        var itens = await consulta.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync(ct);
        return (itens, total);
    }

    public Task<int> ContarAsync(CancellationToken ct = default) => _contexto.Usuarios.CountAsync(ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default)
    {
        await _contexto.Usuarios.AddAsync(usuario, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Usuario usuario, CancellationToken ct = default)
    {
        _contexto.Usuarios.Update(usuario);
        await _contexto.SaveChangesAsync(ct);
    }
}
