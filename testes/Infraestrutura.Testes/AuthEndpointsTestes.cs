using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VeiCards.Aplicacao.Dtos;
using Xunit;

namespace VeiCards.Infraestrutura.Testes;

public class AuthEndpointsTestes : IClassFixture<FabricaApiTestes>
{
    private readonly HttpClient _cliente;

    public AuthEndpointsTestes(FabricaApiTestes fabrica)
    {
        _cliente = fabrica.CreateClient();
    }

    [Fact]
    public async Task Registrar_ComDadosValidos_DeveRetornar200ComToken()
    {
        var requisicao = new RegistrarUsuarioRequest($"user{Guid.NewGuid():N}", $"{Guid.NewGuid():N}@teste.com", "Usuário Teste", "senha123");

        var resposta = await _cliente.PostAsJsonAsync("/api/v1/auth/registrar", requisicao);

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var corpo = await resposta.Content.ReadFromJsonAsync<AutenticacaoResponse>();
        corpo!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Registrar_ComEmailInvalido_DeveRetornar400()
    {
        var requisicao = new RegistrarUsuarioRequest($"user{Guid.NewGuid():N}", "email-invalido", "Usuário Teste", "senha123");

        var resposta = await _cliente.PostAsJsonAsync("/api/v1/auth/registrar", requisicao);

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ComUsuarioInexistente_DeveRetornar422()
    {
        var requisicao = new LoginRequest("usuario-que-nao-existe", "qualquer-senha");

        var resposta = await _cliente.PostAsJsonAsync("/api/v1/auth/login", requisicao);

        resposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Me_SemToken_DeveRetornar401()
    {
        var resposta = await _cliente.GetAsync("/api/v1/auth/me");

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegistrarELogin_FluxoCompleto_DeveAutenticarComSucesso()
    {
        var nomeUsuario = $"user{Guid.NewGuid():N}";
        var registrar = new RegistrarUsuarioRequest(nomeUsuario, $"{Guid.NewGuid():N}@teste.com", "Usuário Teste", "senha123");
        await _cliente.PostAsJsonAsync("/api/v1/auth/registrar", registrar);

        var resposta = await _cliente.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(nomeUsuario, "senha123"));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Registrar_ComNomeUsuarioJaExistente_DeveRetornar422()
    {
        var registrar = new RegistrarUsuarioRequest($"user{Guid.NewGuid():N}", $"{Guid.NewGuid():N}@teste.com", "Usuário Teste", "senha123");
        await _cliente.PostAsJsonAsync("/api/v1/auth/registrar", registrar);

        var resposta = await _cliente.PostAsJsonAsync("/api/v1/auth/registrar", registrar);

        resposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Me_ComTokenValido_DeveRetornarPerfilDoProprioUsuario()
    {
        var nomeUsuario = $"user{Guid.NewGuid():N}";
        var registrar = new RegistrarUsuarioRequest(nomeUsuario, $"{Guid.NewGuid():N}@teste.com", "Usuário Teste", "senha123");
        var respostaRegistrar = await _cliente.PostAsJsonAsync("/api/v1/auth/registrar", registrar);
        var corpo = await respostaRegistrar.Content.ReadFromJsonAsync<AutenticacaoResponse>();

        _cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", corpo!.Token);

        var resposta = await _cliente.GetAsync("/api/v1/auth/me");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var perfil = await resposta.Content.ReadFromJsonAsync<UsuarioResponse>();
        perfil!.NomeUsuario.Should().Be(nomeUsuario);
    }

    [Fact]
    public async Task Me_Put_ComTokenValido_DeveAtualizarPerfil()
    {
        var nomeUsuario = $"user{Guid.NewGuid():N}";
        var registrar = new RegistrarUsuarioRequest(nomeUsuario, $"{Guid.NewGuid():N}@teste.com", "Usuário Teste", "senha123");
        var respostaRegistrar = await _cliente.PostAsJsonAsync("/api/v1/auth/registrar", registrar);
        var corpo = await respostaRegistrar.Content.ReadFromJsonAsync<AutenticacaoResponse>();

        _cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", corpo!.Token);

        var resposta = await _cliente.PutAsJsonAsync("/api/v1/auth/me", new AtualizarPerfilRequest("Nome Atualizado", $"{Guid.NewGuid():N}@teste.com"));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var perfil = await resposta.Content.ReadFromJsonAsync<UsuarioResponse>();
        perfil!.NomeExibicao.Should().Be("Nome Atualizado");
    }
}
