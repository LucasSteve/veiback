namespace VeiCards.Aplicacao.Dtos;

public class ParametrosPaginacao
{
    private const int TamanhoMaximo = 100;
    private int _pagina = 1;
    private int _tamanhoPagina = 20;

    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value switch
        {
            < 1 => 20,
            > TamanhoMaximo => TamanhoMaximo,
            _ => value,
        };
    }
}
