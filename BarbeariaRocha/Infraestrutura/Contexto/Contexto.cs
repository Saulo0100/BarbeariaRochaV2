using BarbeariaRocha.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaRocha.Infraestrutura.Contexto
{
    public class Contexto(DbContextOptions<Contexto> option) : DbContext(option)
    {
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Agendamento> Agendamento { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<CodigoConfirmacao> CodigoConfirmacao { get; set; }
        public DbSet<Excecao> Excecao { get; set; }
        public DbSet<Mensalista> Mensalista { get; set; }
        public DbSet<MensalistaCorte> MensalistaCorte { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Contexto).Assembly);
        }
    }
}
