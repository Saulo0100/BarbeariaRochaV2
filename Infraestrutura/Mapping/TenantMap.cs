using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaRocha.Infraestrutura.Mapping;

public class TenantMap : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Plano)
            .WithMany(p => p.Tenants)
            .HasForeignKey(x => x.PlanoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Dominios)
            .WithOne(d => d.Tenant)
            .HasForeignKey(d => d.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
