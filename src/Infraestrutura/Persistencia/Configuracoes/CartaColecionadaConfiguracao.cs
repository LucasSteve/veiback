using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class CartaColecionadaConfiguracao : IEntityTypeConfiguration<CartaColecionada>
{
    public void Configure(EntityTypeBuilder<CartaColecionada> builder)
    {
        builder.ToTable("cartas_colecionadas");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(c => c.Jogo).HasColumnName("jogo").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(c => c.CartaExternaId).HasColumnName("carta_externa_id").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Numero).HasColumnName("numero").HasMaxLength(20);
        builder.Property(c => c.Raridade).HasColumnName("raridade").HasMaxLength(50);
        builder.Property(c => c.ImagemUrl).HasColumnName("imagem_url").HasMaxLength(500);
        builder.Property(c => c.Tem).HasColumnName("tem").IsRequired();
        builder.Property(c => c.Quero).HasColumnName("quero").IsRequired();
        builder.Property(c => c.Favorito).HasColumnName("favorito").IsRequired();
        builder.Property(c => c.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(c => c.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();

        builder.HasIndex(c => new { c.UsuarioId, c.Jogo, c.CartaExternaId }).IsUnique();
        builder.HasIndex(c => new { c.UsuarioId, c.Jogo });

        builder.HasOne<Usuario>().WithMany().HasForeignKey(c => c.UsuarioId).OnDelete(DeleteBehavior.Cascade);
    }
}
