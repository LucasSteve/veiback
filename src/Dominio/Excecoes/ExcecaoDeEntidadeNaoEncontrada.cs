namespace VeiCards.Dominio.Excecoes;

/// <summary>
/// Lançada quando uma entidade buscada por identificador não existe.
/// A camada de API traduz essa exceção para HTTP 404 via middleware global.
/// </summary>
public class ExcecaoDeEntidadeNaoEncontrada : Exception
{
    public ExcecaoDeEntidadeNaoEncontrada(string nomeEntidade, Guid id)
        : base($"{nomeEntidade} com id '{id}' não foi encontrado(a).")
    {
    }
}
