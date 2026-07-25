using VeiCards.Dominio.Comum;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Dominio.Entidades;

/// <summary>
/// Representa um usuário cadastrado na plataforma (colecionador, jogador ou administrador).
/// </summary>
public class Usuario : EntidadeBase
{
    public string NomeUsuario { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NomeExibicao { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public PapelUsuario Papel { get; private set; }
    public DateTime CriadoEm { get; private set; }

    // Construtor privado exigido pelo EF Core para materializar entidades do banco.
    private Usuario()
    {
    }

    private Usuario(string nomeUsuario, string email, string nomeExibicao, string senhaHash, PapelUsuario papel)
    {
        NomeUsuario = nomeUsuario;
        Email = email;
        NomeExibicao = nomeExibicao;
        SenhaHash = senhaHash;
        Papel = papel;
        CriadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Cria um novo usuário. A senha já deve chegar em domínio como hash — o domínio
    /// não sabe como gerar hash (isso é responsabilidade de infraestrutura, via porta IServicoSenha).
    /// </summary>
    public static Usuario Registrar(string nomeUsuario, string email, string nomeExibicao, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(nomeUsuario) || nomeUsuario.Trim().Length < 3)
        {
            throw new ExcecaoDeRegraDeNegocio("Nome de usuário deve ter ao menos 3 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ExcecaoDeRegraDeNegocio("Email inválido.");
        }

        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new ExcecaoDeRegraDeNegocio("Senha inválida.");
        }

        var nomeExibicaoFinal = string.IsNullOrWhiteSpace(nomeExibicao) ? nomeUsuario : nomeExibicao.Trim();

        return new Usuario(nomeUsuario.Trim(), email.Trim().ToLowerInvariant(), nomeExibicaoFinal, senhaHash, PapelUsuario.Usuario);
    }

    public void AtualizarPerfil(string nomeExibicao, string email)
    {
        if (string.IsNullOrWhiteSpace(nomeExibicao))
        {
            throw new ExcecaoDeRegraDeNegocio("Nome de exibição não pode ser vazio.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ExcecaoDeRegraDeNegocio("Email inválido.");
        }

        NomeExibicao = nomeExibicao.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void PromoverParaAdmin() => Papel = PapelUsuario.Admin;

    public void RebaixarParaUsuario() => Papel = PapelUsuario.Usuario;
}
