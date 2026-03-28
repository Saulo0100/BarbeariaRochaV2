namespace BarbeariaRocha.Modelos.Request.Horario
{
    /// <summary>
    /// Payload para salvar/atualizar a configuração de um dia da semana.
    /// </summary>
    public class ConfiguracaoHorarioSalvarRequest
    {
        /// <summary>
        /// Dia da semana (0=Domingo, 1=Segunda, ..., 6=Sábado).
        /// </summary>
        public int DiaSemana { get; set; }

        public bool Aberto { get; set; }

        /// <summary>
        /// Hora de início no formato "HH:mm". Obrigatório se Aberto = true.
        /// </summary>
        public string? HoraInicio { get; set; }

        /// <summary>
        /// Hora de início do almoço no formato "HH:mm". Nulo = sem intervalo de almoço.
        /// </summary>
        public string? AlmocoInicio { get; set; }

        /// <summary>
        /// Hora de fim do almoço no formato "HH:mm". Nulo = sem intervalo de almoço.
        /// </summary>
        public string? AlmocoFim { get; set; }

        /// <summary>
        /// Hora de fim do expediente no formato "HH:mm". Obrigatório se Aberto = true.
        /// </summary>
        public string? HoraFim { get; set; }

        /// <summary>
        /// Intervalo entre slots em minutos (padrão: 40).
        /// </summary>
        public int IntervaloMinutos { get; set; } = 40;
    }
}
