using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MovimentacaoEstoqueMap : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Motivo)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Tipo)
            .HasConversion<string>();

        builder.Property(x => x.DataMovimentacao)
            .HasColumnType("timestamp without time zone");

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.ProdutoId })
            .HasDatabaseName("IX_MovimentacoesEstoque_TenantId_ProdutoId");
    }
}
