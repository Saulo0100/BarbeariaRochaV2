using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ConfiguracaoSiteMap : IEntityTypeConfiguration<ConfiguracaoSite>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoSite> builder)
    {
        builder.ToTable("ConfiguracaoSite");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Dominio)
            .WithOne()
            .HasForeignKey<ConfiguracaoSite>(x => x.DominioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.NomeSite)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Logo)
            .IsRequired(false);

        builder.Property(x => x.CorPrimaria)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CorSecundaria)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CorFundo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CorTexto)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CorAcento)
            .IsRequired(false)
            .HasMaxLength(20);
    }
}
