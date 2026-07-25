using FluentAssertions;
using Moq;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Aplicacao.Servicos;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;
using Xunit;

namespace VeiCards.Aplicacao.Testes.Servicos;

public class ServicoInscricoesEventosTestes
{
    private readonly Mock<IRepositorioInscricoesEventos> _repositorioInscricoes = new();
    private readonly Mock<IRepositorioEventos> _repositorioEventos = new();
    private readonly ServicoInscricoesEventos _servico;

    public ServicoInscricoesEventosTestes()
    {
        _servico = new ServicoInscricoesEventos(_repositorioInscricoes.Object, _repositorioEventos.Object);
    }

    private static Evento CriarEventoComCapacidade(int capacidade) =>
        Evento.Criar("Torneio", null, DateTime.UtcNow.AddDays(1), null, null, null, null, null, TipoEvento.Torneio, capacidade, null);

    [Fact]
    public async Task InscreverAsync_ComEventoLotado_DeveLancarExcecaoDeRegraDeNegocio()
    {
        var evento = CriarEventoComCapacidade(2);
        var usuarioId = Guid.NewGuid();

        _repositorioEventos.Setup(r => r.ObterPorIdAsync(evento.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evento);
        _repositorioInscricoes.Setup(r => r.ObterAsync(evento.Id, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((InscricaoEvento?)null);
        _repositorioInscricoes.Setup(r => r.ContarPorEventoAsync(evento.Id, It.IsAny<CancellationToken>())).ReturnsAsync(2);

        var acao = () => _servico.InscreverAsync(evento.Id, usuarioId, CancellationToken.None);

        await acao.Should().ThrowAsync<ExcecaoDeRegraDeNegocio>();
        _repositorioInscricoes.Verify(r => r.AdicionarAsync(It.IsAny<InscricaoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InscreverAsync_ComVagasDisponiveis_DeveAdicionarInscricao()
    {
        var evento = CriarEventoComCapacidade(64);
        var usuarioId = Guid.NewGuid();

        _repositorioEventos.Setup(r => r.ObterPorIdAsync(evento.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evento);
        _repositorioInscricoes.Setup(r => r.ObterAsync(evento.Id, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((InscricaoEvento?)null);
        _repositorioInscricoes.Setup(r => r.ContarPorEventoAsync(evento.Id, It.IsAny<CancellationToken>())).ReturnsAsync(10);

        var resultado = await _servico.InscreverAsync(evento.Id, usuarioId, CancellationToken.None);

        resultado.UsuarioId.Should().Be(usuarioId);
        _repositorioInscricoes.Verify(r => r.AdicionarAsync(It.IsAny<InscricaoEvento>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InscreverAsync_QuandoJaInscrito_DeveSerIdempotenteENaoDuplicar()
    {
        var evento = CriarEventoComCapacidade(64);
        var usuarioId = Guid.NewGuid();
        var inscricaoExistente = InscricaoEvento.Criar(evento.Id, usuarioId);

        _repositorioEventos.Setup(r => r.ObterPorIdAsync(evento.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evento);
        _repositorioInscricoes.Setup(r => r.ObterAsync(evento.Id, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(inscricaoExistente);

        var resultado = await _servico.InscreverAsync(evento.Id, usuarioId, CancellationToken.None);

        resultado.UsuarioId.Should().Be(usuarioId);
        _repositorioInscricoes.Verify(r => r.AdicionarAsync(It.IsAny<InscricaoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InscreverAsync_ComInscricoesFechadas_DeveLancarExcecaoDeRegraDeNegocio()
    {
        var evento = CriarEventoComCapacidade(64);
        evento.FecharInscricoes();
        var usuarioId = Guid.NewGuid();

        _repositorioEventos.Setup(r => r.ObterPorIdAsync(evento.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evento);
        _repositorioInscricoes.Setup(r => r.ObterAsync(evento.Id, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((InscricaoEvento?)null);

        var acao = () => _servico.InscreverAsync(evento.Id, usuarioId, CancellationToken.None);

        await acao.Should().ThrowAsync<ExcecaoDeRegraDeNegocio>();
        _repositorioInscricoes.Verify(r => r.AdicionarAsync(It.IsAny<InscricaoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InscreverAsync_ComEventoInexistente_DeveLancarExcecaoDeEntidadeNaoEncontrada()
    {
        var eventoId = Guid.NewGuid();
        _repositorioEventos.Setup(r => r.ObterPorIdAsync(eventoId, It.IsAny<CancellationToken>())).ReturnsAsync((Evento?)null);

        var acao = () => _servico.InscreverAsync(eventoId, Guid.NewGuid(), CancellationToken.None);

        await acao.Should().ThrowAsync<ExcecaoDeEntidadeNaoEncontrada>();
    }

    [Fact]
    public async Task CancelarAsync_SemInscricaoExistente_NaoDeveLancarExcecao()
    {
        var eventoId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        _repositorioInscricoes.Setup(r => r.ObterAsync(eventoId, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((InscricaoEvento?)null);

        var acao = () => _servico.CancelarAsync(eventoId, usuarioId, CancellationToken.None);

        await acao.Should().NotThrowAsync();
        _repositorioInscricoes.Verify(r => r.RemoverAsync(It.IsAny<InscricaoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
