using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

public class ServicoEventos
{
    private readonly IRepositorioEventos _repositorio;
    private readonly IRepositorioInscricoesEventos _repositorioInscricoes;

    public ServicoEventos(IRepositorioEventos repositorio, IRepositorioInscricoesEventos repositorioInscricoes)
    {
        _repositorio = repositorio;
        _repositorioInscricoes = repositorioInscricoes;
    }

    public async Task<ResultadoPaginado<EventoResponse>> ListarAsync(FiltroEventos filtro, CancellationToken ct = default)
    {
        var (itens, total) = await _repositorio.ListarAsync(filtro, ct);
        var respostas = new List<EventoResponse>();
        foreach (var evento in itens)
        {
            respostas.Add(await MapearParaResponseAsync(evento, ct));
        }

        return new ResultadoPaginado<EventoResponse>(respostas, filtro.Pagina, filtro.TamanhoPagina, total);
    }

    public async Task<EventoResponse> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var evento = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Evento), id);
        return await MapearParaResponseAsync(evento, ct);
    }

    public async Task<EventoResponse> CriarAsync(CriarOuAtualizarEventoRequest requisicao, CancellationToken ct = default)
    {
        var tipo = Enum.Parse<TipoEvento>(requisicao.Tipo, ignoreCase: true);
        var evento = Evento.Criar(requisicao.Nome, requisicao.Descricao, requisicao.Data, requisicao.Horario, requisicao.Local, requisicao.Cidade, requisicao.Organizador, requisicao.Formato, tipo, requisicao.Capacidade, requisicao.ImagemUrl);
        await _repositorio.AdicionarAsync(evento, ct);
        return await MapearParaResponseAsync(evento, ct);
    }

    public async Task<EventoResponse> AtualizarAsync(Guid id, CriarOuAtualizarEventoRequest requisicao, CancellationToken ct = default)
    {
        var evento = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Evento), id);
        var tipo = Enum.Parse<TipoEvento>(requisicao.Tipo, ignoreCase: true);
        evento.Atualizar(requisicao.Nome, requisicao.Descricao, requisicao.Data, requisicao.Horario, requisicao.Local, requisicao.Cidade, requisicao.Organizador, requisicao.Formato, tipo, requisicao.Capacidade, requisicao.ImagemUrl);
        await _repositorio.AtualizarAsync(evento, ct);
        return await MapearParaResponseAsync(evento, ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        var evento = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Evento), id);
        await _repositorio.RemoverAsync(evento, ct);
    }

    public async Task<EventoResponse> AtualizarInscricoesAbertasAsync(Guid id, bool abertas, CancellationToken ct = default)
    {
        var evento = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Evento), id);

        if (abertas)
        {
            evento.AbrirInscricoes();
        }
        else
        {
            evento.FecharInscricoes();
        }

        await _repositorio.AtualizarAsync(evento, ct);
        return await MapearParaResponseAsync(evento, ct);
    }

    private async Task<EventoResponse> MapearParaResponseAsync(Evento evento, CancellationToken ct)
    {
        var vagasOcupadas = await _repositorioInscricoes.ContarPorEventoAsync(evento.Id, ct);
        return new EventoResponse(
            evento.Id,
            evento.Nome,
            evento.Descricao,
            evento.Data,
            evento.Horario,
            evento.Local,
            evento.Cidade,
            evento.Organizador,
            evento.Formato,
            evento.Tipo.ToString(),
            evento.CalcularStatus(DateTime.UtcNow).ToString(),
            evento.Capacidade,
            vagasOcupadas,
            evento.ImagemUrl,
            evento.InscricoesAbertas);
    }
}
