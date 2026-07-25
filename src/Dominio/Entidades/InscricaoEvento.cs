using VeiCards.Dominio.Comum;

namespace VeiCards.Dominio.Entidades;

/// <summary>
/// Inscrição de um usuário em um evento. Equivalente server-side do registrationStore
/// que hoje vive só no localStorage do frontend.
/// </summary>
public class InscricaoEvento : EntidadeBase
{
    public Guid EventoId { get; private set; }
    public Guid UsuarioId { get; private set; }
    public DateTime DataInscricao { get; private set; }

    private InscricaoEvento()
    {
    }

    private InscricaoEvento(Guid eventoId, Guid usuarioId)
    {
        EventoId = eventoId;
        UsuarioId = usuarioId;
        DataInscricao = DateTime.UtcNow;
    }

    public static InscricaoEvento Criar(Guid eventoId, Guid usuarioId) => new(eventoId, usuarioId);
}
