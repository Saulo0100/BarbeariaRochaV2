using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaRocha.Infraestrutura.Mapping
{
    public class MensalistaCorteMap : IEntityTypeConfiguration<MensalistaCorte>
    {
        public void Configure(EntityTypeBuilder<MensalistaCorte> builder)
        {
            builder.ToTable("MensalistaCortes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MensalistaId)
                .IsRequired();

            builder.Property(x => x.DataCorte)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.Observacao)
                .HasMaxLength(200);

            builder.HasOne<Mensalista>()
                .WithMany()
                .HasForeignKey(x => x.MensalistaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
