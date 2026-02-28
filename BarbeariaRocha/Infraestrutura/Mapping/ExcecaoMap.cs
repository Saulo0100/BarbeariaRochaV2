using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaRocha.Infraestrutura.Mapping
{
    public class ExcecaoMap : IEntityTypeConfiguration<Excecao>
    {
        public void Configure(EntityTypeBuilder<Excecao> builder)
        {
            builder.ToTable("Excecoes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Data)
                .IsRequired()
                .HasColumnType("date");
            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.BarbeiroId)
                .IsRequired(false);

            builder.Property(x => x.Excluido)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.BarbeiroId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
