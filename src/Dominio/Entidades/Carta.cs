using VeiCards.Dominio.Comum;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Dominio.Entidades;

/// <summary>
/// Carta do catálogo interno da VEI Cards (coleção própria da plataforma).
/// Não deve ser confundida com as cartas consultadas via TCGdex — aquela é uma
/// integração pública de terceiros consumida diretamente pelo frontend, fora do domínio.
/// </summary>
public class Carta : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Numero { get; private set; }
    public string? Expansao { get; private set; }
    public string? Raridade { get; private set; }
    public string? Jogo { get; private set; }
    public string? ImagemUrl { get; private set; }

    private Carta()
    {
    }

    private Carta(string nome, string? numero, string? expansao, string? raridade, string? jogo, string? imagemUrl)
    {
        Nome = nome;
        Numero = numero;
        Expansao = expansao;
        Raridade = raridade;
        Jogo = jogo;
        ImagemUrl = imagemUrl;
    }

    public static Carta Criar(string nome, string? numero, string? expansao, string? raridade, string? jogo, string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ExcecaoDeRegraDeNegocio("Nome da carta é obrigatório.");
        }

        return new Carta(nome.Trim(), numero, expansao, raridade, jogo, imagemUrl);
    }

    public void Atualizar(string nome, string? numero, string? expansao, string? raridade, string? jogo, string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ExcecaoDeRegraDeNegocio("Nome da carta é obrigatório.");
        }

        Nome = nome.Trim();
        Numero = numero;
        Expansao = expansao;
        Raridade = raridade;
        Jogo = jogo;
        ImagemUrl = imagemUrl;
    }
}
