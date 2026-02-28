using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaRocha.Infraestrutura.Mapping
{
    public class MensalistaMap : IEntityTypeConfiguration<Mensalista>
    {
        public void Configure(EntityTypeBuilder<Mensalista> builder)
        {
            builder.ToTable("Mensalistas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Numero)
                .IsRequired()
                .HasMaxLength(11);

            builder.HasIndex(x => x.Numero)
            .IsUnique()
            .HasFilter("\"Status\" = 'Ativo'");

            builder.Property(x => x.Valor)
             .IsRequired();

            builder.Property(x => x.Dia)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Tipo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(10);

        }
    }
}
