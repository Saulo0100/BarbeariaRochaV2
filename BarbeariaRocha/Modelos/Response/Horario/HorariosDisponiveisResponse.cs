namespace BarbeariaRocha.Modelos.Response.Horario
{
    public class HorariosDisponiveisResponse
    {
        public DateTime Data { get; set; }
        public bool Aberto { get; set; }
        public required string DiaSemana { get; set; }
        public List<string> HorariosDisponiveis { get; set; } = new();
        public List<string> HorariosOcupados { get; set; } = new();
    }

    public class HorariosDisponiveisServicoResponse
    {
        public DateTime Data { get; set; }
        public bool Aberto { get; set; }
        public required string DiaSemana { get; set; }
        public List<string> HorariosDisponiveis { get; set; } = new();
        public List<string> HorariosOcupados { get; set; } = new();

        /// <summary>
        /// Para serviços com RequerDuasEtapas: horários disponíveis para a segunda etapa,
        /// dado um horário selecionado na primeira etapa.
        /// </summary>
        public List<string> HorariosDisponiveisEtapa2 { get; set; } = new();

        /// <summary>
        /// Indica se o serviço requer duas etapas
        /// </summary>
        public bool RequerDuasEtapas { get; set; }

        /// <summary>
        /// Intervalo mínimo em horas entre etapas (se aplicável)
        /// </summary>
        public int IntervaloMinimoHoras { get; set; }
    }
}
