using FluentAssertions;
using Moq;
using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Aplicacao.Servicos;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;
using Xunit;

namespace VeiCards.Aplicacao.Testes.Servicos;

public class ServicoAutenticacaoTestes
{
    private readonly Mock<IRepositorioUsuarios> _repositorio = new();
    private readonly Mock<IServicoSenha> _servicoSenha = new();
    private readonly Mock<IServicoToken> _servicoToken = new();
    private readonly ServicoAutenticacao _servico;

    public ServicoAutenticacaoTestes()
    {
        _servico = new ServicoAutenticacao(_repositorio.Object, _servicoSenha.Object, _servicoToken.Object);
    }

    [Fact]
    public async Task RegistrarAsync_ComUsuarioJaExistente_DeveLancarExcecaoDeRegraDeNegocio()
    {
        _repositorio.Setup(r => r.ExisteComNomeUsuarioOuEmailAsync("joao", "joao@teste.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var requisicao = new RegistrarUsuarioRequest("joao", "joao@teste.com", "João", "senha123");

        var acao = () => _servico.RegistrarAsync(requisicao, CancellationToken.None);

        await acao.Should().ThrowAsync<ExcecaoDeRegraDeNegocio>();
        _repositorio.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarAsync_ComDadosValidos_DeveGerarHashEPersistirUsuario()
    {
        _repositorio.Setup(r => r.ExisteComNomeUsuarioOuEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _servicoSenha.Setup(s => s.GerarHash("senha123")).Returns("hash-gerado");
        _servicoToken.Setup(t => t.GerarToken(It.IsAny<Usuario>())).Returns("token-jwt-fake");

        var requisicao = new RegistrarUsuarioRequest("joao", "joao@teste.com", "João", "senha123");

        var resultado = await _servico.RegistrarAsync(requisicao, CancellationToken.None);

        resultado.Token.Should().Be("token-jwt-fake");
        resultado.Usuario.NomeUsuario.Should().Be("joao");
        _repositorio.Verify(r => r.AdicionarAsync(It.Is<Usuario>(u => u.SenhaHash == "hash-gerado"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ComSenhaIncorreta_DeveLancarExcecaoDeRegraDeNegocio()
    {
        var usuario = Usuario.Registrar("joao", "joao@teste.com", "João", "hash-armazenado");
        _repositorio.Setup(r => r.ObterPorNomeUsuarioAsync("joao", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _servicoSenha.Setup(s => s.VerificarHash("senha-errada", "hash-armazenado")).Returns(false);

        var acao = () => _servico.LoginAsync(new LoginRequest("joao", "senha-errada"), CancellationToken.None);

        await acao.Should().ThrowAsync<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public async Task LoginAsync_ComUsuarioInexistente_DeveLancarExcecaoDeRegraDeNegocio()
    {
        _repositorio.Setup(r => r.ObterPorNomeUsuarioAsync("fantasma", It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var acao = () => _servico.LoginAsync(new LoginRequest("fantasma", "qualquer"), CancellationToken.None);

        await acao.Should().ThrowAsync<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public async Task LoginAsync_ComCredenciaisValidas_DeveRetornarToken()
    {
        var usuario = Usuario.Registrar("joao", "joao@teste.com", "João", "hash-armazenado");
        _repositorio.Setup(r => r.ObterPorNomeUsuarioAsync("joao", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _servicoSenha.Setup(s => s.VerificarHash("senha123", "hash-armazenado")).Returns(true);
        _servicoToken.Setup(t => t.GerarToken(usuario)).Returns("token-jwt-fake");

        var resultado = await _servico.LoginAsync(new LoginRequest("joao", "senha123"), CancellationToken.None);

        resultado.Token.Should().Be("token-jwt-fake");
    }
}
