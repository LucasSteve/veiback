using FluentAssertions;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;
using Xunit;

namespace VeiCards.Dominio.Testes.Entidades;

public class EventoTestes
{
    private static Evento CriarEvento(DateTime data) =>
        Evento.Criar("Torneio Teste", "Descrição", data, "18:00", "Local", "Cidade", "Organizador", "Standard", TipoEvento.Torneio, 64, null);

    [Fact]
    public void CalcularStatus_ComDataFutura_DeveRetornarEmBreve()
    {
        var evento = CriarEvento(DateTime.UtcNow.AddDays(5));

        evento.CalcularStatus(DateTime.UtcNow).Should().Be(StatusEvento.EmBreve);
    }

    [Fact]
    public void CalcularStatus_ComDataDeHoje_DeveRetornarAoVivo()
    {
        var evento = CriarEvento(DateTime.UtcNow.Date);

        evento.CalcularStatus(DateTime.UtcNow).Should().Be(StatusEvento.AoVivo);
    }

    [Fact]
    public void CalcularStatus_ComDataPassada_DeveRetornarEncerrado()
    {
        var evento = CriarEvento(DateTime.UtcNow.AddDays(-3));

        evento.CalcularStatus(DateTime.UtcNow).Should().Be(StatusEvento.Encerrado);
    }

    [Fact]
    public void Criar_ComCapacidadeNegativa_DeveLancarExcecao()
    {
        var acao = () => Evento.Criar("Torneio", null, DateTime.UtcNow, null, null, null, null, null, TipoEvento.Torneio, -1, null);

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }

    [Fact]
    public void Criar_SemNome_DeveLancarExcecao()
    {
        var acao = () => Evento.Criar(" ", null, DateTime.UtcNow, null, null, null, null, null, TipoEvento.Torneio, null, null);

        acao.Should().Throw<ExcecaoDeRegraDeNegocio>();
    }
}
