using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AgendamentoProdutoMap : IEntityTypeConfiguration<AgendamentoProduto>
{
    public void Configure(EntityTypeBuilder<AgendamentoProduto> builder)
    {
        builder.ToTable("AgendamentosProdutos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.NomeProduto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PrecoProduto)
            .HasColumnType("numeric(10,2)");

        builder.HasOne<Agendamento>()
            .WithMany()
            .HasForeignKey(x => x.AgendamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
