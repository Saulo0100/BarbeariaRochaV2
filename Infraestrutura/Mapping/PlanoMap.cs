using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaRocha.Infraestrutura.Mapping;

public class PlanoMap : IEntityTypeConfiguration<Plano>
{
    public void Configure(EntityTypeBuilder<Plano> builder)
    {
        builder.ToTable("Planos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MaxBarbeiros)
            .IsRequired();

        builder.Property(x => x.Ativo)
            .HasDefaultValue(true);

        builder.HasData(
            new Plano { Id = 1, Nome = "Básico", MaxBarbeiros = 1, Ativo = true },
            new Plano { Id = 2, Nome = "Profissional", MaxBarbeiros = 3, Ativo = true },
            new Plano { Id = 3, Nome = "Premium", MaxBarbeiros = 5, Ativo = true }
        );
    }
}
