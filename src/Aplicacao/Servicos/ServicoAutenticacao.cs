using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>Casos de uso de autenticação, sessão (access + refresh token) e perfil do usuário logado.</summary>
public class ServicoAutenticacao
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly IRepositorioRefreshTokens _repositorioRefreshTokens;
    private readonly IServicoSenha _servicoSenha;
    private readonly IServicoToken _servicoToken;

    public ServicoAutenticacao(
        IRepositorioUsuarios repositorioUsuarios,
        IRepositorioRefreshTokens repositorioRefreshTokens,
        IServicoSenha servicoSenha,
        IServicoToken servicoToken)
    {
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioRefreshTokens = repositorioRefreshTokens;
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

        return await EmitirSessaoAsync(usuario, ct);
    }

    public async Task<AutenticacaoResponse> LoginAsync(LoginRequest requisicao, CancellationToken ct = default)
    {
        var usuario = await _repositorioUsuarios.ObterPorNomeUsuarioAsync(requisicao.NomeUsuario, ct);
        if (usuario is null || !_servicoSenha.VerificarHash(requisicao.Senha, usuario.SenhaHash))
        {
            throw new ExcecaoDeRegraDeNegocio("Usuário ou senha inválidos.");
        }

        return await EmitirSessaoAsync(usuario, ct);
    }

    /// <summary>
    /// Rotação de refresh token: o token apresentado é revogado e um par novo (access +
    /// refresh) é emitido, mesmo que ainda esteja dentro da validade — evita que um token
    /// roubado continue utilizável indefinidamente sem o dono perceber.
    /// </summary>
    public async Task<AutenticacaoResponse> RenovarSessaoAsync(RefreshTokenRequest requisicao, CancellationToken ct = default)
    {
        var hash = _servicoToken.CalcularHashRefreshToken(requisicao.RefreshToken);
        var tokenArmazenado = await _repositorioRefreshTokens.ObterPorHashAsync(hash, ct);

        if (tokenArmazenado is null || !tokenArmazenado.EstaAtivo)
        {
            throw new ExcecaoDeRegraDeNegocio("Refresh token inválido ou expirado.");
        }

        var usuario = await _repositorioUsuarios.ObterPorIdAsync(tokenArmazenado.UsuarioId, ct)
            ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Usuario), tokenArmazenado.UsuarioId);

        tokenArmazenado.Revogar();
        await _repositorioRefreshTokens.AtualizarAsync(tokenArmazenado, ct);

        return await EmitirSessaoAsync(usuario, ct);
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

    private async Task<AutenticacaoResponse> EmitirSessaoAsync(Usuario usuario, CancellationToken ct)
    {
        var (token, expiraEm) = _servicoToken.GerarTokenDeAcesso(usuario);

        var refreshTokenBruto = _servicoToken.GerarRefreshTokenBruto();
        var refreshTokenHash = _servicoToken.CalcularHashRefreshToken(refreshTokenBruto);
        var refreshToken = RefreshToken.Criar(usuario.Id, refreshTokenHash, DateTime.UtcNow.Add(_servicoToken.DuracaoRefreshToken));
        await _repositorioRefreshTokens.AdicionarAsync(refreshToken, ct);

        return new AutenticacaoResponse(MapearParaResponse(usuario), token, expiraEm, refreshTokenBruto);
    }

    private static UsuarioResponse MapearParaResponse(Usuario usuario) =>
        new(usuario.Id, usuario.NomeUsuario, usuario.Email, usuario.NomeExibicao, usuario.Papel.ToString());
}
