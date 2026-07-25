namespace VeiCards.Dominio.Excecoes;

/// <summary>
/// Lançada quando uma regra de negócio do domínio é violada
/// (ex.: username duplicado, capacidade de evento excedida).
/// A camada de API traduz essa exceção para HTTP 422/400 via middleware global.
/// </summary>
public class ExcecaoDeRegraDeNegocio : Exception
{
    public ExcecaoDeRegraDeNegocio(string mensagem) : base(mensagem)
    {
    }
}
