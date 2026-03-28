using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Servico
{
    public class ServicoCriarRequest
    {
        public required string Nome { get; set; }
        public required decimal Valor { get; set; }
        public required int TempoEstimado { get; set; }
        public required string Descricao { get; set; }
        public CategoriaServico Categoria { get; set; }
    }
}
