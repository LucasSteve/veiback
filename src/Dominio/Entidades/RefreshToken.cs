using VeiCards.Dominio.Comum;

namespace VeiCards.Dominio.Entidades;

/// <summary>
/// Refresh token com rotação: cada uso emite um novo e revoga o atual. Guardamos apenas o
/// hash do token (nunca o valor em texto puro), igual senha — se o banco vazar, os tokens
/// de sessão ativos não vazam junto.
/// </summary>
public class RefreshToken : EntidadeBase
{
    public Guid UsuarioId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CriadoEm { get; private set; }
    public DateTime ExpiraEm { get; private set; }
    public DateTime? RevogadoEm { get; private set; }

    public bool EstaAtivo => RevogadoEm is null && ExpiraEm > DateTime.UtcNow;

    private RefreshToken()
    {
    }

    private RefreshToken(Guid usuarioId, string tokenHash, DateTime expiraEm)
    {
        UsuarioId = usuarioId;
        TokenHash = tokenHash;
        CriadoEm = DateTime.UtcNow;
        ExpiraEm = expiraEm;
    }

    public static RefreshToken Criar(Guid usuarioId, string tokenHash, DateTime expiraEm) =>
        new(usuarioId, tokenHash, expiraEm);

    public void Revogar() => RevogadoEm = DateTime.UtcNow;
}
