namespace BarbeariaRocha.Modelos.Paginacao
{
    public class PaginacaoFiltro<T>
    {
        public int Pagina { get; set; } = 1;
        public int ItensPorPagina { get; set; } = 10;
        public required T Filtro { get; set; }
    }
}
