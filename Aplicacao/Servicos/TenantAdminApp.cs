using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Request.Tenant;
using BarbeariaRocha.Modelos.Response.Tenant;
using Microsoft.EntityFrameworkCore;
using AppContexto = BarbeariaRocha.Infraestrutura.Contexto.Contexto;

namespace BarbeariaRocha.Aplicacao.Servicos;

public class TenantAdminApp(AppContexto contexto) : ITenantAdminApp
{
    public async Task<TenantDetalhesResponse> Criar(TenantCriarRequest request)
    {
        var planoExiste = await contexto.Plano.AnyAsync(p => p.Id == request.PlanoId && p.Ativo);
        if (!planoExiste)
            throw new Exception("Plano não encontrado ou inativo.");

        var dominioJaExiste = await contexto.TenantDominio.AnyAsync(d => d.Dominio == request.Dominio);
        if (dominioJaExiste)
            throw new Exception("Já existe um tenant cadastrado com este domínio.");

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            PlanoId = request.PlanoId
        };

        tenant.Dominios.Add(new TenantDominio
        {
            Dominio = request.Dominio,
            Autorizado = true
        });

        contexto.Tenant.Add(tenant);
        await contexto.SaveChangesAsync();

        var plano = await contexto.Plano.FindAsync(request.PlanoId);

        return new TenantDetalhesResponse
        {
            Id = tenant.Id,
            Nome = tenant.Nome,
            Plano = plano!.Nome,
            MaxBarbeiros = plano.MaxBarbeiros,
            Dominios = tenant.Dominios.Select(d => new TenantDominioResponse
            {
                Dominio = d.Dominio,
                Autorizado = d.Autorizado
            }).ToList()
        };
    }

    public async Task Deletar(string dominio)
    {
        var tenantDominio = await contexto.TenantDominio
            .FirstOrDefaultAsync(d => d.Dominio == dominio)
            ?? throw new Exception("Domínio não encontrado.");

        var tenantId = tenantDominio.TenantId.ToString();

        // Cascade manual nas tabelas de negócio (não possuem FK para Tenant)
        await contexto.AgendamentoAdicional
            .Where(a => contexto.Agendamento
                .Where(ag => ag.TenantId == tenantId)
                .Select(ag => ag.Id)
                .Contains(a.AgendamentoId))
            .ExecuteDeleteAsync();

        await contexto.Agendamento.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.MensalistaCorte.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.Mensalista.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.Servico.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.Adicional.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.CodigoConfirmacao.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.ConfiguracaoHorario.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.ConfiguracaoBarbearia.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.Excecao.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();
        await contexto.Usuario.Where(a => a.TenantId == tenantId).ExecuteDeleteAsync();

        // Remove o Tenant (cascata no DB remove TenantDominios automaticamente)
        await contexto.Tenant
            .Where(t => t.Id == tenantDominio.TenantId)
            .ExecuteDeleteAsync();
    }

    public async Task EditarPlano(Guid tenantId, TenantEditarPlanoRequest request)
    {
        var tenant = await contexto.Tenant.FindAsync(tenantId)
            ?? throw new Exception("Tenant não encontrado.");

        var planoExiste = await contexto.Plano.AnyAsync(p => p.Id == request.PlanoId && p.Ativo);
        if (!planoExiste)
            throw new Exception("Plano não encontrado ou inativo.");

        tenant.PlanoId = request.PlanoId;
        await contexto.SaveChangesAsync();
    }
}
