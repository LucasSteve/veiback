using VeiCards.Dominio.Comum;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Dominio.Entidades;

/// <summary>
/// Uma carta salva na coleção pessoal de um usuário. Guarda um snapshot completo dos
/// dados da carta (nome, número, raridade, imagem) no momento em que foi salva — a carta
/// em si vem de uma API externa por jogo (TCGdex, Scryfall, YGOPRODeck, etc.), não de um
/// catálogo próprio, então a coleção não pode depender de um catálogo local para existir.
/// Isso também evita uma nova consulta externa só para montar a tela "Minha Coleção".
/// </summary>
public class CartaColecionada : EntidadeBase
{
    public Guid UsuarioId { get; private set; }
    public TipoJogo Jogo { get; private set; }
    public string CartaExternaId { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string? Numero { get; private set; }
    public string? Raridade { get; private set; }
    public string? ImagemUrl { get; private set; }
    public bool Tem { get; private set; }
    public bool Quero { get; private set; }
    public bool Favorito { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    private CartaColecionada()
    {
    }

    private CartaColecionada(Guid usuarioId, TipoJogo jogo, string cartaExternaId, string nome, string? numero, string? raridade, string? imagemUrl)
    {
        UsuarioId = usuarioId;
        Jogo = jogo;
        CartaExternaId = cartaExternaId;
        Nome = nome;
        Numero = numero;
        Raridade = raridade;
        ImagemUrl = imagemUrl;
        CriadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }

    public static CartaColecionada Criar(Guid usuarioId, TipoJogo jogo, string cartaExternaId, string nome, string? numero, string? raridade, string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(cartaExternaId))
        {
            throw new ExcecaoDeRegraDeNegocio("Identificador externo da carta é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ExcecaoDeRegraDeNegocio("Nome da carta é obrigatório.");
        }

        return new CartaColecionada(usuarioId, jogo, cartaExternaId.Trim(), nome.Trim(), numero, raridade, imagemUrl);
    }

    /// <summary>Aplica um novo estado de Tenho/Quero/Favorito, atualizando também o snapshot dos dados da carta.</summary>
    public void AtualizarStatus(bool tem, bool quero, bool favorito, string nome, string? numero, string? raridade, string? imagemUrl)
    {
        Tem = tem;
        Quero = quero;
        Favorito = favorito;
        Nome = string.IsNullOrWhiteSpace(nome) ? Nome : nome.Trim();
        Numero = numero ?? Numero;
        Raridade = raridade ?? Raridade;
        ImagemUrl = imagemUrl ?? ImagemUrl;
        AtualizadoEm = DateTime.UtcNow;
    }
}
