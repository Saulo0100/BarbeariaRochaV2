using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Request.Autenticacao;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class AutenticacaoApp(Contexto contexto, TokenProvider token, IEmailApp emailApp) : IAutenticacaoApp
    {
        private readonly TokenProvider _token = token;
        private readonly Contexto _contexto = contexto;
        private readonly IEmailApp _emailApp = emailApp;

        public void AtualizarSenha(int id, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha.Length < 6)
                throw new ArgumentException("A nova senha deve ter pelo menos 6 caracteres.");

            var usuario = _contexto.Set<Usuario>()
                .FirstOrDefault(x => x.Id == id && x.Excluido == false)
                ?? throw new Exception("Usuário não encontrado.");

            usuario.Senha = novaSenha;
            _contexto.SaveChanges();
        }

        public void EsqueceuSenha(EsqueceuSenhaRequest request)
        {
            request.Numero = HelperGenerico.RemoveMask(request.Numero);

            var usuario = _contexto.Set<Usuario>()
                .FirstOrDefault(x => x.Numero == request.Numero && x.Email == request.Email && x.Excluido == false)
                ?? throw new Exception("Usuário não encontrado com esse número e email.");

            var tokenRedefinicao = Guid.NewGuid().ToString();
            usuario.TokenConfirmacao = tokenRedefinicao;
            _contexto.SaveChanges();

            _emailApp.EnviarEmailRedefinicaoSenha(usuario.Email, usuario.Nome, tokenRedefinicao);
        }

        public void RedefinirSenha(RedefinirSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                throw new ArgumentException("Token inválido.");

            if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < 6)
                throw new ArgumentException("A nova senha deve ter pelo menos 6 caracteres.");

            var usuario = _contexto.Set<Usuario>()
                .FirstOrDefault(x => x.TokenConfirmacao == request.Token && x.Excluido == false)
                ?? throw new Exception("Token inválido ou expirado.");

            usuario.Senha = request.NovaSenha;
            usuario.TokenConfirmacao = null;
            _contexto.SaveChanges();
        }

        public string Login(LoginRequest login)
        {
            login.Numero = HelperGenerico.RemoveMask(login.Numero);
            var barbeiro = _contexto.Set<Usuario>()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Numero == login.Numero && x.Excluido == false) ?? throw new Exception("Usuário não encontrado");

            if (login.Senha != barbeiro.Senha)
                throw new Exception("Senha inválida");

            if (!barbeiro.EmailConfirmado && barbeiro.Perfil == "Cliente")
                throw new Exception("Confirme seu email antes de fazer login. Verifique sua caixa de entrada.");

            return _token.CreateToken(barbeiro);
        }
    }
}
