using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");

        builder.Property(u => u.NomeUsuario).HasColumnName("nome_usuario").HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
        builder.Property(u => u.NomeExibicao).HasColumnName("nome_exibicao").HasMaxLength(100).IsRequired();
        builder.Property(u => u.SenhaHash).HasColumnName("senha_hash").HasMaxLength(200).IsRequired();
        builder.Property(u => u.Papel).HasColumnName("papel").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(u => u.CriadoEm).HasColumnName("criado_em").IsRequired();

        builder.HasIndex(u => u.NomeUsuario).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
