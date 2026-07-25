namespace VeiCards.Aplicacao.Dtos;

public record RegistrarUsuarioRequest(string NomeUsuario, string Email, string NomeExibicao, string Senha);

public record LoginRequest(string NomeUsuario, string Senha);

public record AtualizarPerfilRequest(string NomeExibicao, string Email);

public record RefreshTokenRequest(string RefreshToken);

public record UsuarioResponse(Guid Id, string NomeUsuario, string Email, string NomeExibicao, string Papel);

public record AutenticacaoResponse(UsuarioResponse Usuario, string Token, DateTime Expiracao, string RefreshToken);
