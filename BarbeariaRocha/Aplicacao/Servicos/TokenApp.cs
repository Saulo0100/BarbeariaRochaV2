using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class TokenApp(Contexto contexto) : ITokenApp
    {
        private readonly Contexto _contexto = contexto;
        public void GerarToken(string numero)
        {
            var tokenAtivo = _contexto.CodigoConfirmacao
                .FirstOrDefault(t => t.Numero == numero && !t.Confirmado && t.DtExpiracao.ToUniversalTime() > DateTime.UtcNow);

            var ultimoAgendamento = _contexto.Agendamento
                .Where(x => x.NumeroCliente == numero && x.Status == AgendamentoStatus.Concluido.ToString())
                .OrderByDescending(x => x.DataHora)
                .FirstOrDefault();

            if (ultimoAgendamento != null && ultimoAgendamento.DataHora.AddDays(7) >= DateTime.UtcNow)
                throw new Exception("Seu ultimo corte foi a menos de 7 dias");

            if (tokenAtivo != null && tokenAtivo.Reenviado)
                throw new Exception("Já foi enviado um código de confirmação. Por favor, verifique seu telefone.");

            if (tokenAtivo != null && !tokenAtivo.Reenviado)
            {
                HelperGenerico.EnviarMensagem(tokenAtivo.Codigo.ToString(), numero);
                tokenAtivo.Reenviado = true;
                _contexto.SaveChanges();
                return;
            }

            var codigo = HelperGenerico.GerarCodigoConfirmacao();
            var salvarToken = new CodigoConfirmacao(numero, codigo);

            _contexto.CodigoConfirmacao.Add(salvarToken);
            _contexto.SaveChanges();
            HelperGenerico.EnviarMensagem(codigo.ToString(), numero);

        }
    }
}
