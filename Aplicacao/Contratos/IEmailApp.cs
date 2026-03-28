namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IEmailApp
    {
        void EnviarEmailConfirmacao(string email, string nome, string token);
        void EnviarEmailRedefinicaoSenha(string email, string nome, string token);
    }
}
