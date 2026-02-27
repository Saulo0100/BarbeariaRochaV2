using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UsuarioMap : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Numero)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(x => x.Numero)
        .IsUnique()
        .HasFilter("\"Excluido\" = false");

        builder.Property(x => x.Email)
            .HasMaxLength(50);

        builder.Property(x => x.Senha)
            .IsRequired();

        builder.Property(x => x.Perfil)
            .IsRequired();


    }
}