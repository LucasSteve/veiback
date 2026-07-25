using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>
/// Casos de uso de inscrição em eventos — equivalente server-side do registrationStore do frontend.
/// </summary>
public class ServicoInscricoesEventos
{
    private readonly IRepositorioInscricoesEventos _repositorioInscricoes;
    private readonly IRepositorioEventos _repositorioEventos;

    public ServicoInscricoesEventos(IRepositorioInscricoesEventos repositorioInscricoes, IRepositorioEventos repositorioEventos)
    {
        _repositorioInscricoes = repositorioInscricoes;
        _repositorioEventos = repositorioEventos;
    }

    public async Task<InscricaoEventoResponse> InscreverAsync(Guid eventoId, Guid usuarioId, CancellationToken ct = default)
    {
        var evento = await _repositorioEventos.ObterPorIdAsync(eventoId, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Evento), eventoId);

        var jaInscrito = await _repositorioInscricoes.ObterAsync(eventoId, usuarioId, ct);
        if (jaInscrito is not null)
        {
            return new InscricaoEventoResponse(jaInscrito.EventoId, jaInscrito.UsuarioId, jaInscrito.DataInscricao);
        }

        if (evento.Capacidade is { } capacidade)
        {
            var vagasOcupadas = await _repositorioInscricoes.ContarPorEventoAsync(eventoId, ct);
            if (vagasOcupadas >= capacidade)
            {
                throw new ExcecaoDeRegraDeNegocio("Este evento já atingiu a capacidade máxima de inscrições.");
            }
        }

        var inscricao = InscricaoEvento.Criar(eventoId, usuarioId);
        await _repositorioInscricoes.AdicionarAsync(inscricao, ct);

        return new InscricaoEventoResponse(inscricao.EventoId, inscricao.UsuarioId, inscricao.DataInscricao);
    }

    public async Task CancelarAsync(Guid eventoId, Guid usuarioId, CancellationToken ct = default)
    {
        var inscricao = await _repositorioInscricoes.ObterAsync(eventoId, usuarioId, ct);
        if (inscricao is null)
        {
            return;
        }

        await _repositorioInscricoes.RemoverAsync(inscricao, ct);
    }

    public async Task<IReadOnlyList<InscricaoEventoResponse>> ListarMinhasAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var inscricoes = await _repositorioInscricoes.ListarPorUsuarioAsync(usuarioId, ct);
        return inscricoes.Select(i => new InscricaoEventoResponse(i.EventoId, i.UsuarioId, i.DataInscricao)).ToList();
    }
}
