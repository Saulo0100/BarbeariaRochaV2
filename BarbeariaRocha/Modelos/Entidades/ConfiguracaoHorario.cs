namespace BarbeariaRocha.Modelos.Entidades
{
    /// <summary>
    /// Configuração de horários de funcionamento por dia da semana.
    /// Uma linha por dia (0=Domingo, 1=Segunda, ..., 6=Sábado).
    /// </summary>
    public class ConfiguracaoHorario
    {
        public int Id { get; set; }

        /// <summary>
        /// Dia da semana (0=Domingo, 1=Segunda, ..., 6=Sábado).
        /// </summary>
        public int DiaSemana { get; set; }

        /// <summary>
        /// Se a barbearia está aberta neste dia.
        /// </summary>
        public bool Aberto { get; set; }

        /// <summary>
        /// Hora de início do expediente. Nulo se fechado.
        /// </summary>
        public TimeOnly? HoraInicio { get; set; }

        /// <summary>
        /// Hora de início do almoço. Nulo se não houver intervalo de almoço.
        /// </summary>
        public TimeOnly? AlmocoInicio { get; set; }

        /// <summary>
        /// Hora de fim do almoço / retorno ao trabalho. Nulo se não houver intervalo.
        /// </summary>
        public TimeOnly? AlmocoFim { get; set; }

        /// <summary>
        /// Hora de fim do expediente. Nulo se fechado.
        /// </summary>
        public TimeOnly? HoraFim { get; set; }

        /// <summary>
        /// Intervalo entre slots de agendamento em minutos (padrão: 40).
        /// </summary>
        public int IntervaloMinutos { get; set; } = 40;
    }
}
