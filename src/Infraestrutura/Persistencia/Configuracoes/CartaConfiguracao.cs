using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class CartaConfiguracao : IEntityTypeConfiguration<Carta>
{
    public void Configure(EntityTypeBuilder<Carta> builder)
    {
        builder.ToTable("cartas");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Numero).HasColumnName("numero").HasMaxLength(20);
        builder.Property(c => c.Expansao).HasColumnName("expansao").HasMaxLength(100);
        builder.Property(c => c.Raridade).HasColumnName("raridade").HasMaxLength(50);
        builder.Property(c => c.Jogo).HasColumnName("jogo").HasMaxLength(50);
        builder.Property(c => c.ImagemUrl).HasColumnName("imagem_url").HasMaxLength(500);

        builder.HasIndex(c => c.Jogo);
        builder.HasIndex(c => c.Raridade);
        builder.HasIndex(c => c.Nome);
    }
}
