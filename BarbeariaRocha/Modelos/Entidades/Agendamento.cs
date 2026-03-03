using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Request.Agendamento;
using System.Diagnostics.CodeAnalysis;

namespace BarbeariaRocha.Modelos.Entidades
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public int BarbeiroId { get; set; }
        public int? ServicoId { get; set; }
        public required string NomeCliente { get; set; }
        public required string NumeroCliente { get; set; }
        public required string Status { get; set; }
        public string? MetodoPagamento { get; set; }
        public DateTime DataHora { get; set; }

        /// <summary>
        /// Referência ao agendamento principal (para serviços com 2 etapas).
        /// A etapa 2 aponta para a etapa 1.
        /// </summary>
        public int? AgendamentoPrincipalId { get; set; }

        /// <summary>
        /// Descrição da etapa (ex: "Platinado", "Corte"). Null para serviços normais.
        /// </summary>
        public string? DescricaoEtapa { get; set; }

        public Agendamento() { }

        [SetsRequiredMembers]
        public Agendamento(AgendamentoCriarRequest request)
        {
            ValidarCamposObrigatorios(request);
            Usuario.ValidarNumero(request.Numero);
            ValidarBarbeiroId(request.BarbeiroId);

            if (request.UsuarioId != 0)
                UsuarioId = request.UsuarioId;
            NomeCliente = request.Nome;
            NumeroCliente = request.Numero;
            BarbeiroId = request.BarbeiroId;
            DataHora = request.DtAgendamento;
            Status = AgendamentoStatus.Pendente.ToString();
            ServicoId = request.ServicoId;
        }

        private static void ValidarCamposObrigatorios(AgendamentoCriarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("O nome do cliente é obrigatório.", nameof(request.Nome));

            if (string.IsNullOrWhiteSpace(request.Numero))
                throw new ArgumentException("O número do cliente é obrigatório.", nameof(request.Numero));
        }

        private static void ValidarBarbeiroId(int barbeiroId)
        {
            if (barbeiroId <= 0)
                throw new ArgumentException("O ID do barbeiro é obrigatório e deve ser válido.", nameof(barbeiroId));
        }
    }
}
