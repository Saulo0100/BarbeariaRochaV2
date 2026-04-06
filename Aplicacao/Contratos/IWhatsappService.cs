namespace BarbeariaRocha.Aplicacao.Contratos;

public interface IWhatsappService
{
    Task EnviarMensagemAsync(string codigo, string numeroDestino);
}
