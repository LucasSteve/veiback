using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>Casos de uso de autenticação e perfil do usuário logado.</summary>
public class ServicoAutenticacao
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly IServicoSenha _servicoSenha;
    private readonly IServicoToken _servicoToken;

    public ServicoAutenticacao(IRepositorioUsuarios repositorioUsuarios, IServicoSenha servicoSenha, IServicoToken servicoToken)
    {
        _repositorioUsuarios = repositorioUsuarios;
        _servicoSenha = servicoSenha;
        _servicoToken = servicoToken;
    }

    public async Task<AutenticacaoResponse> RegistrarAsync(RegistrarUsuarioRequest requisicao, CancellationToken ct = default)
    {
        var jaExiste = await _repositorioUsuarios.ExisteComNomeUsuarioOuEmailAsync(requisicao.NomeUsuario, requisicao.Email, ct);
        if (jaExiste)
        {
            throw new ExcecaoDeRegraDeNegocio("Já existe um usuário com esse nome de usuário ou email.");
        }

        var senhaHash = _servicoSenha.GerarHash(requisicao.Senha);
        var usuario = Usuario.Registrar(requisicao.NomeUsuario, requisicao.Email, requisicao.NomeExibicao, senhaHash);

        await _repositorioUsuarios.AdicionarAsync(usuario, ct);

        return MontarResposta(usuario);
    }

    public async Task<AutenticacaoResponse> LoginAsync(LoginRequest requisicao, CancellationToken ct = default)
    {
        var usuario = await _repositorioUsuarios.ObterPorNomeUsuarioAsync(requisicao.NomeUsuario, ct);
        if (usuario is null || !_servicoSenha.VerificarHash(requisicao.Senha, usuario.SenhaHash))
        {
            throw new ExcecaoDeRegraDeNegocio("Usuário ou senha inválidos.");
        }

        return MontarResposta(usuario);
    }

    public async Task<UsuarioResponse> ObterPerfilAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var usuario = await _repositorioUsuarios.ObterPorIdAsync(usuarioId, ct)
            ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Usuario), usuarioId);

        return MapearParaResponse(usuario);
    }

    public async Task<UsuarioResponse> AtualizarPerfilAsync(Guid usuarioId, AtualizarPerfilRequest requisicao, CancellationToken ct = default)
    {
        var usuario = await _repositorioUsuarios.ObterPorIdAsync(usuarioId, ct)
            ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Usuario), usuarioId);

        usuario.AtualizarPerfil(requisicao.NomeExibicao, requisicao.Email);
        await _repositorioUsuarios.AtualizarAsync(usuario, ct);

        return MapearParaResponse(usuario);
    }

    private AutenticacaoResponse MontarResposta(Usuario usuario) =>
        new(MapearParaResponse(usuario), _servicoToken.GerarToken(usuario));

    private static UsuarioResponse MapearParaResponse(Usuario usuario) =>
        new(usuario.Id, usuario.NomeUsuario, usuario.Email, usuario.NomeExibicao, usuario.Papel.ToString());
}
