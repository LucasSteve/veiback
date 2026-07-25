using VeiCards.Dominio.Comum;

namespace VeiCards.Dominio.Entidades;

/// <summary>
/// Relação entre um Usuário e uma Carta: "Tenho", "Quero" e "Favorito".
/// Equivalente server-side do que hoje vive só no localStorage do frontend (collectionStore).
/// </summary>
public class StatusCartaUsuario : EntidadeBase
{
    public Guid UsuarioId { get; private set; }
    public Guid CartaId { get; private set; }
    public bool Tem { get; private set; }
    public bool Quero { get; private set; }
    public bool Favorito { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    private StatusCartaUsuario()
    {
    }

    private StatusCartaUsuario(Guid usuarioId, Guid cartaId)
    {
        UsuarioId = usuarioId;
        CartaId = cartaId;
        AtualizadoEm = DateTime.UtcNow;
    }

    public static StatusCartaUsuario Criar(Guid usuarioId, Guid cartaId) => new(usuarioId, cartaId);

    /// <summary>Aplica um novo estado completo (tenho/quero/favorito), vindo de um toggle da UI.</summary>
    public void AtualizarStatus(bool tem, bool quero, bool favorito)
    {
        Tem = tem;
        Quero = quero;
        Favorito = favorito;
        AtualizadoEm = DateTime.UtcNow;
    }
}
