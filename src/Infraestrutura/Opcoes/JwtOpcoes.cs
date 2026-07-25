namespace VeiCards.Infraestrutura.Opcoes;

/// <summary>Configuração de emissão de token JWT, vinda de appsettings/variáveis de ambiente (seção "Jwt").</summary>
public class JwtOpcoes
{
    public const string Secao = "Jwt";

    public string ChaveSecreta { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int ExpiracaoEmMinutos { get; set; } = 60 * 24;
}
