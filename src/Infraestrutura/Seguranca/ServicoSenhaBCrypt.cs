using VeiCards.Aplicacao.Portas;

namespace VeiCards.Infraestrutura.Seguranca;

public class ServicoSenhaBCrypt : IServicoSenha
{
    public string GerarHash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12);

    public bool VerificarHash(string senha, string hash) => BCrypt.Net.BCrypt.Verify(senha, hash);
}
