using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Request.Autenticacao;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class AutenticacaoApp(Contexto contexto, TokenProvider token) : IAutenticacaoApp
    {
        private readonly TokenProvider _token = token;
        private readonly Contexto _contexto = contexto;
        public void AtualizarSenha(int id, string novaSenha)
        {
            throw new NotImplementedException();
        }

        public void EsqueceuSenha(EsqueceuSenhaRequest request)
        {
            throw new NotImplementedException();
        }

        public string Login(LoginRequest login)
        {
            login.Numero = HelperGenerico.RemoveMask(login.Numero);
            var barbeiro = _contexto.Set<Usuario>()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Numero == login.Numero) ?? throw new Exception("Usuário não encontrado");

            if (login.Senha == barbeiro.Senha)
                return _token.CreateToken(barbeiro);
            else
                throw new Exception("Senha inválida");
        }
    }
}
