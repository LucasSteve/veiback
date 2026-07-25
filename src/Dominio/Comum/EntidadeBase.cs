namespace VeiCards.Dominio.Comum;

/// <summary>
/// Classe base para todas as entidades do domínio. Centraliza a identidade (Id)
/// e a igualdade por identidade, comportamento padrão de qualquer Entidade em DDD.
/// </summary>
public abstract class EntidadeBase
{
    public Guid Id { get; protected set; }

    protected EntidadeBase()
    {
        Id = Guid.NewGuid();
    }

    protected EntidadeBase(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("O identificador da entidade não pode ser vazio.", nameof(id));
        }

        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not EntidadeBase outra || GetType() != outra.GetType())
        {
            return false;
        }

        return Id == outra.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}
