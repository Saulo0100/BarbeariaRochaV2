using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AgendamentoMap : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NomeCliente)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.NumeroCliente)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.DataHora)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.MetodoPagamento)
            .HasConversion<string>();

        builder.HasOne<Servico>()
           .WithMany()
           .HasForeignKey(x => x.ServicoId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
           .WithMany()
           .HasForeignKey(x => x.UsuarioId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(x => x.BarbeiroId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}