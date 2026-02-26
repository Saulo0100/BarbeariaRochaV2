using BarbeariaRocha.Modelos.Request.Autenticacao;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IAutenticacaoApp
    {
        string Login(LoginRequest login);
        void EsqueceuSenha(EsqueceuSenhaRequest request);
        void AtualizarSenha(int id, string novaSenha);
    }
}
