using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class InscricaoEventoConfiguracao : IEntityTypeConfiguration<InscricaoEvento>
{
    public void Configure(EntityTypeBuilder<InscricaoEvento> builder)
    {
        builder.ToTable("inscricoes_eventos");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");

        builder.Property(i => i.EventoId).HasColumnName("evento_id").IsRequired();
        builder.Property(i => i.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(i => i.DataInscricao).HasColumnName("data_inscricao").IsRequired();

        builder.HasIndex(i => new { i.EventoId, i.UsuarioId }).IsUnique();

        builder.HasOne<Evento>().WithMany().HasForeignKey(i => i.EventoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Usuario>().WithMany().HasForeignKey(i => i.UsuarioId).OnDelete(DeleteBehavior.Cascade);
    }
}
