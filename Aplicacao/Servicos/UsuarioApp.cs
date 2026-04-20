using BarbeariaRocha.Infraestrutura.Excecoes;
﻿using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Usuario;
using BarbeariaRocha.Modelos.Response.Barbeiro;
using BarbeariaRocha.Modelos.Response.Usuario;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class UsuarioApp(Contexto contexto, IEmailApp emailApp, ITenantService tenantService) : IUsuarioApp
    {
        public readonly Contexto _contexto = contexto;
        private readonly IEmailApp _emailApp = emailApp;
        private readonly ITenantService _tenantService = tenantService;

        public void Criar(UsuarioCriarRequest request)
        {
            // Apenas clientes podem se auto-cadastrar
            if (request.Perfil != Perfil.Cliente)
                throw new AppException("Apenas clientes podem se cadastrar.");

            var tenantId = _tenantService.ObterTenantId();

            var existente = _contexto.Usuario.FirstOrDefault(u => u.TenantId == tenantId && u.Numero == request.Numero && !u.Excluido);
            if (existente != null)
                throw new AppException("Já existe um usuário com este número.");

            var usuario = new Usuario(request);
            usuario.TenantId = tenantId;
            _contexto.Usuario.Add(usuario);
            _contexto.SaveChanges();

            // Enviar email de confirmação
            try
            {
                var dominio = _contexto.TenantDominio.Where(t => t.TenantId == new Guid(tenantId)).Select(x => x.Dominio).FirstOrDefault();
                var nomeEstabelecimento = _contexto.Tenant.Where(t => t.Id == new Guid(tenantId)).Select(x => x.Nome).FirstOrDefault();
                if (dominio != null)
                    _emailApp.EnviarEmailConfirmacao(usuario.Email, usuario.Nome, usuario.TokenConfirmacao!, dominio, nomeEstabelecimento!);
            }
            catch
            {
                throw new AppException("Usuário criado, mas falha ao enviar email de confirmação. Por favor, entre em contato com o suporte.");
            }
        }

        public void CriarComoAdmin(UsuarioCriarRequest request)
        {
            var tenantId = _tenantService.ObterTenantId();

            var existente = _contexto.Usuario.FirstOrDefault(u => u.TenantId == tenantId && u.Numero == request.Numero && !u.Excluido);
            if (existente != null)
                throw new AppException("Já existe um usuário com este número.");

            var perfisBarbeiro = new[] { Perfil.Barbeiro.ToString(), Perfil.BarbeiroAdministrador.ToString() };
            if (perfisBarbeiro.Contains(request.Perfil.ToString()) && Guid.TryParse(tenantId, out var tenantGuid))
            {
                var tenant = _contexto.Tenant
                    .Include(t => t.Plano)
                    .FirstOrDefault(t => t.Id == tenantGuid)
                    ?? throw new AppException("Tenant não encontrado.");

                var totalBarbeiros = _contexto.Usuario.Count(u =>
                    u.TenantId == tenantId &&
                    perfisBarbeiro.Contains(u.Perfil) &&
                    !u.Excluido);

                if (totalBarbeiros >= tenant.Plano.MaxBarbeiros)
                    throw new AppException("Seu plano atual não permite cadastrar mais barbeiros.");
            }

            var usuario = new Usuario(request);
            usuario.TenantId = tenantId;

            // Usuário criado pelo barbeiro/admin já tem email confirmado automaticamente
            usuario.EmailConfirmado = true;
            usuario.TokenConfirmacao = null;

            // Salvar porcentagem do admin se informada (apenas para barbeiros)
            if (request.Porcentagem.HasValue && request.Porcentagem.Value > 0)
                usuario.Porcentagem = request.Porcentagem.Value;

            // Salvar período de trabalho se informado (apenas para barbeiros)
            if (request.PeriodoTrabalho.HasValue)
            {
                var periodo = (PeriodoTrabalho)request.PeriodoTrabalho.Value;
                if (Enum.IsDefined(typeof(PeriodoTrabalho), periodo))
                    usuario.PeriodoTrabalho = periodo.ToString();
            }

            _contexto.Usuario.Add(usuario);
            _contexto.SaveChanges();
        }

        public void ConfirmarEmail(string token)
        {
            var tenantId = _tenantService.ObterTenantId();
            var usuario = _contexto.Usuario.FirstOrDefault(u => u.TenantId == tenantId && u.TokenConfirmacao == token && !u.Excluido)
                ?? throw new AppException("Token inválido ou expirado.");

            usuario.EmailConfirmado = true;
            usuario.TokenConfirmacao = null;
            _contexto.SaveChanges();
        }

        public void Editar(int id, UsuarioEditarRequest request)
        {
            var usuario = _contexto.Usuario.Find(id) ?? throw new AppException("Usuário não encontrado.");

            if (!string.IsNullOrWhiteSpace(request.Nome))
                usuario.Nome = request.Nome;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                Usuario.ValidarEmail(request.Email);
                usuario.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Numero))
            {
                Usuario.ValidarNumero(request.Numero);
                usuario.Numero = request.Numero;
            }

            if (!string.IsNullOrWhiteSpace(request.Descricao))
                usuario.Descricao = request.Descricao;

            if (!string.IsNullOrWhiteSpace(request.Agenda?.ToString()))
                usuario.Agenda = request.Agenda?.ToString();

            if (!string.IsNullOrWhiteSpace(request.Foto))
                usuario.Foto = Convert.FromBase64String(request.Foto);

            _contexto.SaveChanges();
        }

        public void Excluir(int id)
        {
            var usuario = _contexto.Usuario.Find(id) ?? throw new AppException("Usuário não encontrado.");
            usuario.Excluido = true;
            _contexto.SaveChanges();
        }

        public void EditarPorcentagem(int id, decimal porcentagem)
        {
            var tenantId = _tenantService.ObterTenantId();
            var usuario = _contexto.Usuario.FirstOrDefault(u => u.Id == id && u.TenantId == tenantId) ?? throw new AppException("Usuário não encontrado.");
            if (porcentagem < 0 || porcentagem > 100)
                throw new ArgumentException("A porcentagem deve estar entre 0 e 100.");

            var porcentagemAntiga = usuario.Porcentagem ?? 0;
            var agendamentosSemHistorico = _contexto.Agendamento
                .Where(a => a.TenantId == tenantId
                    && a.BarbeiroId == id
                    && a.Status == "Concluido"
                    && a.PorcentagemAdminNaEpoca == null)
                .ToList();

            foreach (var ag in agendamentosSemHistorico)
            {
                ag.PorcentagemAdminNaEpoca = porcentagemAntiga;
            }

            usuario.Porcentagem = porcentagem;
            _contexto.SaveChanges();
        }

        public IEnumerable<BarbeirosDetalhesResponse> ObterBarbeiros()
        {
            var tenantId = _tenantService.ObterTenantId();
            var perfisBarbeiro = new[] { Perfil.Barbeiro.ToString(), Perfil.BarbeiroAdministrador.ToString() };
            var barbeiros = _contexto.Usuario.Where(u => u.TenantId == tenantId && perfisBarbeiro.Contains(u.Perfil) && !u.Excluido).ToList();
            return barbeiros.Select(b => new BarbeirosDetalhesResponse
            {
                Id = b.Id,
                Nome = b.Nome,
                Descricao = b.Descricao,
                Agenda = b.Agenda!,
                Foto = b.Foto != null ? Convert.ToBase64String(b.Foto) : null
            });
        }

        public UsuarioDetalhesResponse ObterPorId(int id)
        {
            var usuario = _contexto.Usuario.Find(id) ?? throw new AppException("Usuário não encontrado.");

            return new UsuarioDetalhesResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Descricao = usuario.Descricao,
                Numero = usuario.Numero,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Foto = usuario.Foto,
                Agenda = usuario.Agenda,
            };
        }

        public PaginacaoResultado<UsuarioDetalhesResponse> ObterTodos(PaginacaoFiltro<UsuarioFiltroRequest> filtro)
        {
            var tenantId = _tenantService.ObterTenantId();
            var query = _contexto.Usuario.Where(u => u.TenantId == tenantId && !u.Excluido).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Filtro?.Nome))
                query = query.Where(u => u.Nome.ToUpper().Contains(filtro.Filtro.Nome.ToUpper()));

            if (!string.IsNullOrWhiteSpace(filtro.Filtro?.Perfil))
                query = query.Where(u => u.Perfil == filtro.Filtro.Perfil);

            var totalRegistros = query.Count();

            var usuarios = query
                .Skip((filtro.Pagina - 1) * filtro.ItensPorPagina)
                .Take(filtro.ItensPorPagina)
                .Select(u => new UsuarioDetalhesResponse
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Descricao = u.Descricao,
                    Numero = u.Numero,
                    Email = u.Email,
                    Perfil = u.Perfil,
                    Foto = u.Foto,
                    Agenda = u.Agenda
                })
                .ToList();

            return new PaginacaoResultado<UsuarioDetalhesResponse>
            {
                Items = usuarios,
                TotalRegistros = totalRegistros,
                PaginaAtual = filtro.Pagina,
                ItensPorPagina = filtro.ItensPorPagina
            };
        }
    }
}
