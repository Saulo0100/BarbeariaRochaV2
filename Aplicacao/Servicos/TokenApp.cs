using BarbeariaRocha.Infraestrutura.Excecoes;
﻿using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Modelos.Entidades;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class TokenApp(Contexto contexto, ITenantService tenantService, IWhatsappService whatsapp) : ITokenApp
    {
        private readonly Contexto _contexto = contexto;
        private readonly ITenantService _tenantService = tenantService;
        private readonly IWhatsappService _whatsapp = whatsapp;

        public void GerarToken(string numero)
        {
            var tenantId = _tenantService.ObterTenantId();

            var tokenAtivo = _contexto.CodigoConfirmacao
                .FirstOrDefault(t => t.TenantId == tenantId && t.Numero == numero && !t.Confirmado && t.DtExpiracao.ToUniversalTime() > DateTime.UtcNow);

            if (tokenAtivo != null && tokenAtivo.Reenviado)
                throw new AppException("Já foi enviado um código de confirmação. Por favor, verifique seu telefone.");

            if (tokenAtivo != null && !tokenAtivo.Reenviado)
            {
                _whatsapp.EnviarMensagemAsync(tokenAtivo.Codigo.ToString(), numero).GetAwaiter().GetResult();
                tokenAtivo.Reenviado = true;
                _contexto.SaveChanges();
                return;
            }

            var codigo = HelperGenerico.GerarCodigoConfirmacao();
            var salvarToken = new CodigoConfirmacao(numero, codigo) { TenantId = tenantId };

            _contexto.CodigoConfirmacao.Add(salvarToken);
            _contexto.SaveChanges();
            _whatsapp.EnviarMensagemAsync(codigo.ToString(), numero).GetAwaiter().GetResult();
        }
    }
}
