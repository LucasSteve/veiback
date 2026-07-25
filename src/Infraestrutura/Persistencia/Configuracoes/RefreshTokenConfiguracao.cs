using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class RefreshTokenConfiguracao : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(r => r.TokenHash).HasColumnName("token_hash").HasMaxLength(200).IsRequired();
        builder.Property(r => r.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(r => r.ExpiraEm).HasColumnName("expira_em").IsRequired();
        builder.Property(r => r.RevogadoEm).HasColumnName("revogado_em");

        builder.Ignore(r => r.EstaAtivo);

        builder.HasIndex(r => r.TokenHash).IsUnique();
        builder.HasIndex(r => r.UsuarioId);

        builder.HasOne<Usuario>().WithMany().HasForeignKey(r => r.UsuarioId).OnDelete(DeleteBehavior.Cascade);
    }
}
