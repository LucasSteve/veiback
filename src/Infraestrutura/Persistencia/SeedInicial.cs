using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VeiCards.Aplicacao.Portas;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia;

/// <summary>
/// Garante a existência de um usuário administrador inicial. Idempotente: se "admin" já
/// existir (mesmo já promovido/rebaixado manualmente depois), não faz nada.
/// </summary>
public static class SeedInicial
{
    private const string NomeUsuarioAdmin = "admin";
    private const string EmailAdmin = "admin@veicards.com.br";
    private const string SenhaAdminPadrao = "Abc#123";

    public static async Task AplicarAsync(IServiceProvider servicos)
    {
        using var escopo = servicos.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<VeiCardsDbContext>();
        var servicoSenha = escopo.ServiceProvider.GetRequiredService<IServicoSenha>();

        var jaExiste = await contexto.Usuarios.AnyAsync(u => u.NomeUsuario == NomeUsuarioAdmin);
        if (jaExiste)
        {
            return;
        }

        var senhaHash = servicoSenha.GerarHash(SenhaAdminPadrao);
        var admin = Usuario.Registrar(NomeUsuarioAdmin, EmailAdmin, "Administrador", senhaHash);
        admin.PromoverParaAdmin();

        contexto.Usuarios.Add(admin);
        await contexto.SaveChangesAsync();
    }
}
