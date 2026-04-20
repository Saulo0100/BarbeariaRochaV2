using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProdutoMap : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Descricao)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Preco)
            .HasColumnType("numeric(10,2)");

        builder.HasIndex(x => new { x.TenantId, x.Nome })
            .HasDatabaseName("IX_Produtos_TenantId_Nome");
    }
}
