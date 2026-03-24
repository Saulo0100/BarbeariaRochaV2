using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DominioAutorizadoMap : IEntityTypeConfiguration<DominioAutorizado>
{
    public void Configure(EntityTypeBuilder<DominioAutorizado> builder)
    {
        builder.ToTable("DominiosAutorizados");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Dominio)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.Dominio)
            .IsUnique();

        builder.Property(x => x.Ativo)
            .HasDefaultValue(true);
    }
}
