namespace BarbeariaRocha.Modelos.Request.Relatorio
{
    public class RelatorioFiltroRequest
    {
        public int? BarbeiroId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}
