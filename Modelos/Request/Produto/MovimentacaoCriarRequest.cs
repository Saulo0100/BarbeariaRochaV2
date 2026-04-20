using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Produto
{
    public class MovimentacaoCriarRequest
    {
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public required string Motivo { get; set; }
    }
}
