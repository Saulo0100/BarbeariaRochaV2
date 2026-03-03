using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ServicoMap : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("Servicos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Descricao)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Valor)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(x => x.TempoEstimado)
            .HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

        builder.Property(x => x.RequerDuasEtapas)
            .HasDefaultValue(false);

        builder.Property(x => x.IntervaloMinimoHoras)
            .HasDefaultValue(0);

        builder.Property(x => x.DescricaoEtapa1)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.DescricaoEtapa2)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}
