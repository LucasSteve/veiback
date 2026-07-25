using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Configuracoes;

public class NoticiaConfiguracao : IEntityTypeConfiguration<Noticia>
{
    public void Configure(EntityTypeBuilder<Noticia> builder)
    {
        builder.ToTable("noticias");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasColumnName("id");

        builder.Property(n => n.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
        builder.Property(n => n.Resumo).HasColumnName("resumo").HasMaxLength(500);
        builder.Property(n => n.Conteudo).HasColumnName("conteudo");
        builder.Property(n => n.Categoria).HasColumnName("categoria").HasMaxLength(50);
        builder.Property(n => n.AutorId).HasColumnName("autor_id");
        builder.Property(n => n.DataPublicacao).HasColumnName("data_publicacao").IsRequired();
        builder.Property(n => n.TempoLeituraMinutos).HasColumnName("tempo_leitura_minutos");
        builder.Property(n => n.ImagemUrl).HasColumnName("imagem_url").HasMaxLength(500);

        builder.HasIndex(n => n.Categoria);
        builder.HasIndex(n => n.AutorId);

        builder.HasOne<Usuario>().WithMany().HasForeignKey(n => n.AutorId).OnDelete(DeleteBehavior.Restrict);
    }
}
