using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AgendamentoMap : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);

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

        builder.Property(x => x.AgendamentoPrincipalId)
            .IsRequired(false);

        builder.Property(x => x.DescricaoEtapa)
            .HasMaxLength(100)
            .IsRequired(false);

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

        builder.HasOne<Agendamento>()
               .WithMany()
               .HasForeignKey(x => x.AgendamentoPrincipalId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.HasIndex(x => new { x.TenantId, x.BarbeiroId, x.DataHora })
            .HasDatabaseName("IX_Agendamentos_TenantId_BarbeiroId_DataHora");

        builder.HasIndex(x => new { x.TenantId, x.UsuarioId })
            .HasDatabaseName("IX_Agendamentos_TenantId_UsuarioId");

        builder.HasIndex(x => new { x.TenantId, x.Status })
            .HasDatabaseName("IX_Agendamentos_TenantId_Status");
    }
}
