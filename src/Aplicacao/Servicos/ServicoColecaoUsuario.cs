using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>
/// Casos de uso da coleção pessoal do usuário (Tenho/Quero/Favorito por carta) —
/// equivalente server-side do collectionStore do frontend.
/// </summary>
public class ServicoColecaoUsuario
{
    private readonly IRepositorioStatusCartaUsuario _repositorioStatus;
    private readonly IRepositorioCartas _repositorioCartas;

    public ServicoColecaoUsuario(IRepositorioStatusCartaUsuario repositorioStatus, IRepositorioCartas repositorioCartas)
    {
        _repositorioStatus = repositorioStatus;
        _repositorioCartas = repositorioCartas;
    }

    public async Task<IReadOnlyList<StatusCartaResponse>> ObterStatusDoUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var status = await _repositorioStatus.ListarPorUsuarioAsync(usuarioId, ct);
        return status.Select(s => new StatusCartaResponse(s.CartaId, s.Tem, s.Quero, s.Favorito)).ToList();
    }

    public async Task<StatusCartaResponse> AtualizarStatusAsync(Guid usuarioId, Guid cartaId, AtualizarStatusCartaRequest requisicao, CancellationToken ct = default)
    {
        _ = await _repositorioCartas.ObterPorIdAsync(cartaId, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Carta), cartaId);

        var status = await _repositorioStatus.ObterAsync(usuarioId, cartaId, ct);
        if (status is null)
        {
            status = StatusCartaUsuario.Criar(usuarioId, cartaId);
            status.AtualizarStatus(requisicao.Tem, requisicao.Quero, requisicao.Favorito);
            await _repositorioStatus.AdicionarAsync(status, ct);
        }
        else
        {
            status.AtualizarStatus(requisicao.Tem, requisicao.Quero, requisicao.Favorito);
            await _repositorioStatus.AtualizarAsync(status, ct);
        }

        return new StatusCartaResponse(status.CartaId, status.Tem, status.Quero, status.Favorito);
    }
}
