namespace BarbeariaRocha.Modelos.Entidades
{
    public class Servico
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public bool Excluido { get; set; } = false;
        public required string Categoria { get; set; }

        /// <summary>
        /// Indica se o serviço requer duas etapas separadas (ex: Platinado com Corte).
        /// </summary>
        public bool RequerDuasEtapas { get; set; } = false;

        /// <summary>
        /// Intervalo mínimo em horas entre as duas etapas (ex: 4 horas para Platinado com Corte).
        /// </summary>
        public int IntervaloMinimoHoras { get; set; } = 0;

        /// <summary>
        /// Descrição da primeira etapa (ex: "Platinado").
        /// </summary>
        public string? DescricaoEtapa1 { get; set; }

        /// <summary>
        /// Descrição da segunda etapa (ex: "Corte").
        /// </summary>
        public string? DescricaoEtapa2 { get; set; }
    }
}
