namespace BarbeariaRocha.Modelos.Paginacao
{
    public class PaginacaoResultado<T>
    {
        public List<T>? Items { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
    }
}
