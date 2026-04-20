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
        public DbSet<AgendamentoAdicional> AgendamentoAdicional { get; set; }
        public DbSet<Adicional> Adicional { get; set; }
        public DbSet<ConfiguracaoHorario> ConfiguracaoHorario { get; set; }
        public DbSet<ConfiguracaoBarbearia> ConfiguracaoBarbearia { get; set; }
        public DbSet<Plano> Plano { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<TenantDominio> TenantDominio { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacaoEstoque { get; set; }
        public DbSet<AgendamentoProduto> AgendamentoProduto { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Contexto).Assembly);
        }
    }
}
