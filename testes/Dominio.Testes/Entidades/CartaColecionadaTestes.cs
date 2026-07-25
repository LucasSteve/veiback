using FluentAssertions;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;
using Xunit;

namespace VeiCards.Dominio.Testes.Entidades;

public class CartaColecionadaTestes
{
    [Fact]
    public void Criar_ComIdExternoVazio_DeveLancarExcecao()
    {
        var acao = () => CartaColecionada.Criar(Guid.NewGuid(), TipoJogo.Pokemon, "", "Charizard", "006", "Rare Holo", "http://img");

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public void Criar_ComNomeVazio_DeveLancarExcecao()
    {
        var acao = () => CartaColecionada.Criar(Guid.NewGuid(), TipoJogo.Pokemon, "base1-4", "", "006", "Rare Holo", "http://img");

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public void Criar_ComDadosValidos_DevePreencherSnapshotCompleto()
    {
        var carta = CartaColecionada.Criar(Guid.NewGuid(), TipoJogo.Pokemon, "base1-4", "Charizard", "006", "Rare Holo", "http://img");

        carta.Nome.Should().Be("Charizard");
        carta.Jogo.Should().Be(TipoJogo.Pokemon);
        carta.Tem.Should().BeFalse();
    }

    [Fact]
    public void AtualizarStatus_DeveAplicarNovoEstado()
    {
        var carta = CartaColecionada.Criar(Guid.NewGuid(), TipoJogo.Magic, "lea-1", "Black Lotus", null, "Rare", null);

        carta.AtualizarStatus(tem: true, quero: false, favorito: true, "Black Lotus", null, "Rare", "http://nova-imagem");

        carta.Tem.Should().BeTrue();
        carta.Favorito.Should().BeTrue();
        carta.ImagemUrl.Should().Be("http://nova-imagem");
    }
}
