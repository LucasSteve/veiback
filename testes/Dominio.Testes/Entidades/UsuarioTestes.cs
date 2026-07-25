using FluentAssertions;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;
using Xunit;

namespace VeiCards.Dominio.Testes.Entidades;

public class UsuarioTestes
{
    [Fact]
    public void Registrar_ComDadosValidos_DeveCriarUsuarioComPapelUsuario()
    {
        var usuario = Usuario.Registrar("colecionador1", "teste@exemplo.com", "Colecionador", "hash-fake");

        usuario.NomeUsuario.Should().Be("colecionador1");
        usuario.Email.Should().Be("teste@exemplo.com");
        usuario.Papel.Should().Be(Enums.PapelUsuario.Usuario);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Registrar_ComNomeUsuarioMuitoCurto_DeveLancarExcecao(string nomeUsuario)
    {
        var acao = () => Usuario.Registrar(nomeUsuario, "teste@exemplo.com", "Nome", "hash-fake");

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public void Registrar_ComEmailInvalido_DeveLancarExcecao()
    {
        var acao = () => Usuario.Registrar("colecionador1", "email-invalido", "Nome", "hash-fake");

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public void Registrar_SemNomeExibicao_DeveUsarNomeUsuarioComoFallback()
    {
        var usuario = Usuario.Registrar("colecionador1", "teste@exemplo.com", "  ", "hash-fake");

        usuario.NomeExibicao.Should().Be("colecionador1");
    }

    [Fact]
    public void AtualizarPerfil_ComEmailInvalido_DeveLancarExcecao()
    {
        var usuario = Usuario.Registrar("colecionador1", "teste@exemplo.com", "Nome", "hash-fake");

        var acao = () => usuario.AtualizarPerfil("Novo Nome", "invalido");

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }
}
