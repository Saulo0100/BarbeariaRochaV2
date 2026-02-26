using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BarbeariaRocha.Infraestrutura.Contexto
{
    public class ContextoFactory : IDesignTimeDbContextFactory<Contexto>
    {
        public Contexto CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<Contexto>();

            string? connection = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connection))
                throw new InvalidOperationException("Connection não configurada.");

            optionsBuilder.UseNpgsql(connection);

            return new Contexto(optionsBuilder.Options);
        }
    }
}