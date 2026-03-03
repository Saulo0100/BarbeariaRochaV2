namespace BarbeariaRocha.Modelos.Response.Agendamento
{
    public class AgendamentoDetalheResponse
    {
        public int Id { get; set; }
        public required string NomeCliente { get; set; }
        public required string NomeBarbeiro { get; set; }
        public required string NumeroCliente { get; set; }
        public required string Status { get; set; }
        public DateTime Data { get; set; }
        public required string Servico { get; set; }

        /// <summary>
        /// Descrição da etapa (ex: "Platinado", "Corte"). Null para serviços normais.
        /// </summary>
        public string? DescricaoEtapa { get; set; }

        /// <summary>
        /// ID do agendamento principal (null para etapa 1 ou serviços normais).
        /// </summary>
        public int? AgendamentoPrincipalId { get; set; }
    }
}
