namespace VeiCards.Dominio.Enums;

/// <summary>
/// Status calculado (não persistido) de um evento, derivado de Data/Horário
/// em relação ao momento da consulta. Ver <see cref="Entidades.Evento.CalcularStatus"/>.
/// </summary>
public enum StatusEvento
{
    EmBreve = 0,
    AoVivo = 1,
    Encerrado = 2,
}
