using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

public class ServicoNoticias
{
    private readonly IRepositorioNoticias _repositorio;
    private readonly IRepositorioUsuarios _repositorioUsuarios;

    public ServicoNoticias(IRepositorioNoticias repositorio, IRepositorioUsuarios repositorioUsuarios)
    {
        _repositorio = repositorio;
        _repositorioUsuarios = repositorioUsuarios;
    }

    public async Task<ResultadoPaginado<NoticiaResponse>> ListarAsync(FiltroNoticias filtro, CancellationToken ct = default)
    {
        var (itens, total) = await _repositorio.ListarAsync(filtro, ct);
        var respostas = new List<NoticiaResponse>();
        foreach (var noticia in itens)
        {
            respostas.Add(await MapearParaResponseAsync(noticia, ct));
        }

        return new ResultadoPaginado<NoticiaResponse>(respostas, filtro.Pagina, filtro.TamanhoPagina, total);
    }

    public async Task<NoticiaResponse> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var noticia = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Noticia), id);
        return await MapearParaResponseAsync(noticia, ct);
    }

    public async Task<NoticiaResponse> CriarAsync(Guid autorId, CriarOuAtualizarNoticiaRequest requisicao, CancellationToken ct = default)
    {
        var noticia = Noticia.Criar(requisicao.Titulo, requisicao.Resumo, requisicao.Conteudo, requisicao.Categoria, autorId, requisicao.DataPublicacao, requisicao.TempoLeituraMinutos, requisicao.ImagemUrl);
        await _repositorio.AdicionarAsync(noticia, ct);
        return await MapearParaResponseAsync(noticia, ct);
    }

    public async Task<NoticiaResponse> AtualizarAsync(Guid id, CriarOuAtualizarNoticiaRequest requisicao, CancellationToken ct = default)
    {
        var noticia = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Noticia), id);
        noticia.Atualizar(requisicao.Titulo, requisicao.Resumo, requisicao.Conteudo, requisicao.Categoria, requisicao.TempoLeituraMinutos, requisicao.ImagemUrl);
        await _repositorio.AtualizarAsync(noticia, ct);
        return await MapearParaResponseAsync(noticia, ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        var noticia = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Noticia), id);
        await _repositorio.RemoverAsync(noticia, ct);
    }

    private async Task<NoticiaResponse> MapearParaResponseAsync(Noticia noticia, CancellationToken ct)
    {
        string? nomeAutor = null;
        if (noticia.AutorId is { } autorId)
        {
            var autor = await _repositorioUsuarios.ObterPorIdAsync(autorId, ct);
            nomeAutor = autor?.NomeExibicao;
        }

        return new NoticiaResponse(noticia.Id, noticia.Titulo, noticia.Resumo, noticia.Conteudo, noticia.Categoria, nomeAutor, noticia.DataPublicacao, noticia.TempoLeituraMinutos, noticia.ImagemUrl);
    }
}
