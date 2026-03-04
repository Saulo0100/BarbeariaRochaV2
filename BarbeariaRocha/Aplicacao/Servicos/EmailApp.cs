using BarbeariaRocha.Aplicacao.Contratos;
using System.Net;
using System.Net.Mail;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class EmailApp(IConfiguration configuration) : IEmailApp
    {
        private readonly IConfiguration _configuration = configuration;

        public void EnviarEmailConfirmacao(string email, string nome, string token)
        {
            var dominio = _configuration["Proprietario:Dominio"]
                ?? throw new InvalidOperationException("Dominio não configurado.");

            var linkConfirmacao = $"{dominio}/confirmar-email?token={token}";

            var assunto = "Confirme seu cadastro - Barbearia Rocha";
            var corpo = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; background-color: #1a1a1a; color: #e0e0e0; padding: 30px; border-radius: 10px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h1 style='color: #c8a45a; font-size: 24px; margin: 0;'>Barbearia Rocha</h1>
                    </div>
                    <p style='font-size: 16px;'>Olá, <strong>{nome}</strong>!</p>
                    <p style='font-size: 14px; line-height: 1.6;'>
                        Obrigado por se cadastrar na Barbearia Rocha. Para ativar sua conta e começar a agendar seus cortes, confirme seu email clicando no botão abaixo:
                    </p>
                    <div style='text-align: center; margin: 25px 0;'>
                        <a href='{linkConfirmacao}' 
                           style='display: inline-block; background: linear-gradient(135deg, #c8a45a, #a07830); color: #1a1a1a; text-decoration: none; padding: 12px 30px; border-radius: 6px; font-weight: bold; font-size: 14px;'>
                            Confirmar Email
                        </a>
                    </div>
                    <p style='font-size: 12px; color: #888; text-align: center;'>
                        Se você não criou esta conta, ignore este email.
                    </p>
                </div>";

            Enviar(email, assunto, corpo);
        }

        private void Enviar(string destinatario, string assunto, string corpo)
        {
            var from = _configuration["Email:From"]
                ?? throw new InvalidOperationException("Email:From não configurado.");
            var host = _configuration["Email:Host"]
                ?? throw new InvalidOperationException("Email:Host não configurado.");
            var username = _configuration["Email:Username"]
                ?? throw new InvalidOperationException("Email:Username não configurado.");
            var password = _configuration["Email:Password"]
                ?? throw new InvalidOperationException("Email:Password não configurado.");

            using var message = new MailMessage(from, destinatario)
            {
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = true
            };

            using var client = new SmtpClient(host, 587)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            client.Send(message);
        }
    }
}
