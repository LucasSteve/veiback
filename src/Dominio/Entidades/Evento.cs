using VeiCards.Dominio.Comum;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Dominio.Entidades;

public class Evento : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public DateTime Data { get; private set; }
    public string? Horario { get; private set; }
    public string? Local { get; private set; }
    public string? Cidade { get; private set; }
    public string? Organizador { get; private set; }
    public string? Formato { get; private set; }
    public TipoEvento Tipo { get; private set; }
    public int? Capacidade { get; private set; }
    public string? ImagemUrl { get; private set; }

    private Evento()
    {
    }

    private Evento(string nome, string? descricao, DateTime data, string? horario, string? local, string? cidade, string? organizador, string? formato, TipoEvento tipo, int? capacidade, string? imagemUrl)
    {
        Nome = nome;
        Descricao = descricao;
        Data = data;
        Horario = horario;
        Local = local;
        Cidade = cidade;
        Organizador = organizador;
        Formato = formato;
        Tipo = tipo;
        Capacidade = capacidade;
        ImagemUrl = imagemUrl;
    }

    public static Evento Criar(string nome, string? descricao, DateTime data, string? horario, string? local, string? cidade, string? organizador, string? formato, TipoEvento tipo, int? capacidade, string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ExcecaoDeRegraDeNegocio("Nome do evento é obrigatório.");
        }

        if (capacidade is < 0)
        {
            throw new ExcecaoDeRegraDeNegocio("Capacidade do evento não pode ser negativa.");
        }

        return new Evento(nome.Trim(), descricao, data, horario, local, cidade, organizador, formato, tipo, capacidade, imagemUrl);
    }

    public void Atualizar(string nome, string? descricao, DateTime data, string? horario, string? local, string? cidade, string? organizador, string? formato, TipoEvento tipo, int? capacidade, string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ExcecaoDeRegraDeNegocio("Nome do evento é obrigatório.");
        }

        if (capacidade is < 0)
        {
            throw new ExcecaoDeRegraDeNegocio("Capacidade do evento não pode ser negativa.");
        }

        Nome = nome.Trim();
        Descricao = descricao;
        Data = data;
        Horario = horario;
        Local = local;
        Cidade = cidade;
        Organizador = organizador;
        Formato = formato;
        Tipo = tipo;
        Capacidade = capacidade;
        ImagemUrl = imagemUrl;
    }

    /// <summary>
    /// Status é calculado a partir de Data/Horário em relação a "agora", nunca persistido —
    /// evita que um evento fique com status "ao vivo" congelado para sempre no banco.
    /// Considera "ao vivo" o próprio dia do evento; "encerrado" qualquer dia anterior a hoje.
    /// </summary>
    public StatusEvento CalcularStatus(DateTime agoraUtc)
    {
        var dataEvento = Data.Date;
        var hoje = agoraUtc.Date;

        if (dataEvento < hoje)
        {
            return StatusEvento.Encerrado;
        }

        if (dataEvento == hoje)
        {
            return StatusEvento.AoVivo;
        }

        return StatusEvento.EmBreve;
    }
}
