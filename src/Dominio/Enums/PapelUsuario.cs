namespace VeiCards.Dominio.Enums;

/// <summary>
/// Papéis persistidos de um usuário autenticado. "Visitante" (não logado) não é
/// um papel persistido — é apenas a ausência de autenticação, tratada na API.
/// </summary>
public enum PapelUsuario
{
    Usuario = 0,
    Admin = 1,
}
