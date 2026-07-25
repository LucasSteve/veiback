using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class EventoConfiguracao : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("eventos");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Descricao).HasColumnName("descricao");
        builder.Property(e => e.Data).HasColumnName("data").IsRequired();
        builder.Property(e => e.Horario).HasColumnName("horario").HasMaxLength(10);
        builder.Property(e => e.Local).HasColumnName("local").HasMaxLength(200);
        builder.Property(e => e.Cidade).HasColumnName("cidade").HasMaxLength(100);
        builder.Property(e => e.Organizador).HasColumnName("organizador").HasMaxLength(150);
        builder.Property(e => e.Formato).HasColumnName("formato").HasMaxLength(100);
        builder.Property(e => e.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.Capacidade).HasColumnName("capacidade");
        builder.Property(e => e.ImagemUrl).HasColumnName("imagem_url").HasMaxLength(500);

        builder.HasIndex(e => e.Cidade);
        builder.HasIndex(e => e.Tipo);
        builder.HasIndex(e => e.Data);
    }
}
