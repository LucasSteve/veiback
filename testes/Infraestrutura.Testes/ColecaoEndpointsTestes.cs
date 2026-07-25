using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using VeiCards.Aplicacao.Dtos;
using Xunit;

namespace VeiCards.Infraestrutura.Testes;

public class ColecaoEndpointsTestes : IClassFixture<FabricaApiTestes>
{
    private readonly HttpClient _cliente;

    public ColecaoEndpointsTestes(FabricaApiTestes fabrica)
    {
        _cliente = fabrica.CreateClient();
    }

    private async Task AutenticarAsync()
    {
        var registrar = new RegistrarUsuarioRequest($"user{Guid.NewGuid():N}", $"{Guid.NewGuid():N}@teste.com", "Usuário Teste", "senha123");
        var resposta = await _cliente.PostAsJsonAsync("/api/v1/autenticacao/registrar", registrar);
        var corpo = await resposta.Content.ReadFromJsonAsync<AutenticacaoResponse>();
        _cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", corpo!.Token);
    }

    [Fact]
    public async Task ListarJogos_SemAutenticacao_DeveRetornar401()
    {
        var resposta = await _cliente.GetAsync("/api/v1/colecao/jogos");

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SalvarCarta_ComTemVerdadeiro_DeveApareceNaListagemDoJogo()
    {
        await AutenticarAsync();

        var requisicao = new AtualizarStatusColecaoRequest("Charizard", "006", "Rare Holo", "http://img", Tem: true, Quero: false, Favorito: true);
        var respostaSalvar = await _cliente.PutAsJsonAsync("/api/v1/colecao/Pokemon/base1-4", requisicao);
        respostaSalvar.StatusCode.Should().Be(HttpStatusCode.OK);

        var respostaJogos = await _cliente.GetAsync("/api/v1/colecao/jogos");
        var jogos = await respostaJogos.Content.ReadFromJsonAsync<List<JogoComContagemResponse>>();
        jogos.Should().ContainSingle(j => j.Jogo == "Pokemon" && j.Quantidade == 1);

        var respostaLista = await _cliente.GetAsync("/api/v1/colecao/Pokemon");
        var pagina = await respostaLista.Content.ReadFromJsonAsync<ResultadoPaginado<CartaColecionadaResponse>>();
        pagina!.Itens.Should().ContainSingle(c => c.Nome == "Charizard" && c.Tem && c.Favorito);
    }

    [Fact]
    public async Task SalvarCarta_ComJogoInvalido_DeveRetornar422()
    {
        await AutenticarAsync();

        var requisicao = new AtualizarStatusColecaoRequest("Carta", null, null, null, Tem: true, Quero: false, Favorito: false);
        var resposta = await _cliente.PutAsJsonAsync("/api/v1/colecao/JogoQueNaoExiste/id-1", requisicao);

        resposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
