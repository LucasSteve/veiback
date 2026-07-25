using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class StatusCartaUsuarioConfiguracao : IEntityTypeConfiguration<StatusCartaUsuario>
{
    public void Configure(EntityTypeBuilder<StatusCartaUsuario> builder)
    {
        builder.ToTable("status_cartas_usuario");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(s => s.CartaId).HasColumnName("carta_id").IsRequired();
        builder.Property(s => s.Tem).HasColumnName("tem").IsRequired();
        builder.Property(s => s.Quero).HasColumnName("quero").IsRequired();
        builder.Property(s => s.Favorito).HasColumnName("favorito").IsRequired();
        builder.Property(s => s.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();

        builder.HasIndex(s => new { s.UsuarioId, s.CartaId }).IsUnique();

        builder.HasOne<Usuario>().WithMany().HasForeignKey(s => s.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Carta>().WithMany().HasForeignKey(s => s.CartaId).OnDelete(DeleteBehavior.Cascade);
    }
}
