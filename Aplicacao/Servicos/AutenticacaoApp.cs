using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Request.Autenticacao;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class AutenticacaoApp(Contexto contexto, TokenProvider token, IEmailApp emailApp, ITenantService tenantService) : IAutenticacaoApp
    {
        private readonly TokenProvider _token = token;
        private readonly Contexto _contexto = contexto;
        private readonly IEmailApp _emailApp = emailApp;
        private readonly ITenantService _tenantService = tenantService;

        public void AtualizarSenha(int id, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha.Length < 6)
                throw new ArgumentException("A nova senha deve ter pelo menos 6 caracteres.");

            var tenantId = _tenantService.ObterTenantId();
            var usuario = _contexto.Set<Usuario>()
                .FirstOrDefault(x => x.TenantId == tenantId && x.Id == id && x.Excluido == false)
                ?? throw new Exception("Usuário não encontrado.");

            usuario.Senha = novaSenha;
            _contexto.SaveChanges();
        }

        public void EsqueceuSenha(EsqueceuSenhaRequest request)
        {
            request.Numero = HelperGenerico.RemoveMask(request.Numero);

            var tenantId = _tenantService.ObterTenantId();
            var usuario = _contexto.Set<Usuario>()
                .FirstOrDefault(x => x.TenantId == tenantId && x.Numero == request.Numero && x.Email == request.Email && x.Excluido == false)
                ?? throw new Exception("Usuário não encontrado com esse número e email.");

            var tokenRedefinicao = Guid.NewGuid().ToString();
            usuario.TokenConfirmacao = tokenRedefinicao;
            _contexto.SaveChanges();

            try
            {
                var dominio = _contexto.TenantDominio.Where(t => t.TenantId == new Guid(tenantId)).Select(x => x.Dominio).FirstOrDefault();
                var nomeEstabelecimento = _contexto.Tenant.Where(t => t.Id == new Guid(tenantId)).Select(x => x.Nome).FirstOrDefault();
                _emailApp.EnviarEmailRedefinicaoSenha(usuario.Email, usuario.Nome, tokenRedefinicao, dominio!, nomeEstabelecimento!);
            }
            catch
            {

            }
        }

        public void RedefinirSenha(RedefinirSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                throw new ArgumentException("Token inválido.");

            if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < 6)
                throw new ArgumentException("A nova senha deve ter pelo menos 6 caracteres.");

            var tenantId2 = _tenantService.ObterTenantId();
            var usuario = _contexto.Set<Usuario>()
                .FirstOrDefault(x => x.TenantId == tenantId2 && x.TokenConfirmacao == request.Token && x.Excluido == false)
                ?? throw new Exception("Token inválido ou expirado.");

            usuario.Senha = request.NovaSenha;
            usuario.TokenConfirmacao = null;
            _contexto.SaveChanges();
        }

        public string Login(LoginRequest login)
        {
            login.Numero = HelperGenerico.RemoveMask(login.Numero);
            var tenantId3 = _tenantService.ObterTenantId();
            var barbeiro = _contexto.Set<Usuario>()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.TenantId == tenantId3 && x.Numero == login.Numero && x.Excluido == false) ?? throw new Exception("Usuário não encontrado");

            if (login.Senha != barbeiro.Senha)
                throw new Exception("Senha inválida");

            if (!barbeiro.EmailConfirmado && barbeiro.Perfil == "Cliente")
                throw new Exception("Confirme seu email antes de fazer login. Verifique sua caixa de entrada.");

            return _token.CreateToken(barbeiro);
        }
    }
}
