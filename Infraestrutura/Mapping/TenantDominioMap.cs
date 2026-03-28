using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaRocha.Infraestrutura.Mapping;

public class TenantDominioMap : IEntityTypeConfiguration<TenantDominio>
{
    public void Configure(EntityTypeBuilder<TenantDominio> builder)
    {
        builder.ToTable("TenantDominios");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Dominio)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(x => x.Dominio)
            .IsUnique();

        builder.Property(x => x.Autorizado)
            .HasDefaultValue(false);
    }
}
