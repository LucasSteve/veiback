namespace VeiCards.Aplicacao.Portas;

/// <summary>
/// Porta que expõe a identidade do usuário autenticado na requisição atual, sem que a
/// Aplicação precise conhecer HttpContext/claims — implementada na camada Api.
/// </summary>
public interface IUsuarioAutenticado
{
    Guid? UsuarioId { get; }
    bool EstaAutenticado { get; }
    bool EhAdmin { get; }
}
