namespace VeiCards.Dominio.Enums;

/// <summary>
/// Jogos (TCGs) suportados pela coleção. Nomes internos ficam neutros/em inglês
/// (ex.: Pokemon) — rótulos de exibição regionais (ex.: "Pokédex") são responsabilidade
/// exclusiva do frontend, para não acoplar apresentação a domínio.
/// </summary>
public enum TipoJogo
{
    Pokemon = 0,
    YuGiOh = 1,
    Magic = 2,
    OnePiece = 3,
    Lorcana = 4,
    Digimon = 5,
    FleshAndBlood = 6,
    DragonBallSuper = 7,
    StarWarsUnlimited = 8,
    Altered = 9,
    Outro = 10,
}
