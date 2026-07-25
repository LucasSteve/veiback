using FluentAssertions;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;
using Xunit;

namespace VeiCards.Dominio.Testes.Entidades;

public class CartaTestes
{
    [Fact]
    public void Criar_ComNomeVazio_DeveLancarExcecao()
    {
        var acao = () => Carta.Criar("", null, null, null, null, null);

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public void Criar_ComDadosValidos_DevePreencherTodosOsCampos()
    {
        var carta = Carta.Criar("Charizard", "006", "Base", "Rare Holo", "Pokemon", "http://img");

        carta.Nome.Should().Be("Charizard");
        carta.Raridade.Should().Be("Rare Holo");
    }
}
