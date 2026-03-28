using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Servico
{
    public class ServicoFiltroRequest
    {
        public string? Nome { get; set; }
        public CategoriaServico Categoria { get; set; }
    }
}
