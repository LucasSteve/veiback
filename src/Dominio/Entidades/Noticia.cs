using VeiCards.Dominio.Comum;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Dominio.Entidades;

public class Noticia : EntidadeBase
{
    public string Titulo { get; private set; } = string.Empty;
    public string? Resumo { get; private set; }
    public string? Conteudo { get; private set; }
    public string? Categoria { get; private set; }
    public Guid? AutorId { get; private set; }
    public DateTime DataPublicacao { get; private set; }
    public int? TempoLeituraMinutos { get; private set; }
    public string? ImagemUrl { get; private set; }

    private Noticia()
    {
    }

    private Noticia(string titulo, string? resumo, string? conteudo, string? categoria, Guid? autorId, DateTime dataPublicacao, int? tempoLeituraMinutos, string? imagemUrl)
    {
        Titulo = titulo;
        Resumo = resumo;
        Conteudo = conteudo;
        Categoria = categoria;
        AutorId = autorId;
        DataPublicacao = dataPublicacao;
        TempoLeituraMinutos = tempoLeituraMinutos;
        ImagemUrl = imagemUrl;
    }

    public static Noticia Criar(string titulo, string? resumo, string? conteudo, string? categoria, Guid? autorId, DateTime? dataPublicacao, int? tempoLeituraMinutos, string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ExcecaoDeRegraDeNegocio("Título da notícia é obrigatório.");
        }

        return new Noticia(titulo.Trim(), resumo, conteudo, categoria, autorId, dataPublicacao ?? DateTime.UtcNow, tempoLeituraMinutos, imagemUrl);
    }

    public void Atualizar(string titulo, string? resumo, string? conteudo, string? categoria, int? tempoLeituraMinutos, string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ExcecaoDeRegraDeNegocio("Título da notícia é obrigatório.");
        }

        Titulo = titulo.Trim();
        Resumo = resumo;
        Conteudo = conteudo;
        Categoria = categoria;
        TempoLeituraMinutos = tempoLeituraMinutos;
        ImagemUrl = imagemUrl;
    }
}
