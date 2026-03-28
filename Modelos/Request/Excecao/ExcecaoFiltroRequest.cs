namespace BarbeariaRocha.Modelos.Request.Excecao
{
    public class ExcecaoFiltroRequest
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? BarbeiroId { get; set; }
    }
}
