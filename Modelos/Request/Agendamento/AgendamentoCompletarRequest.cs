using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Request.Produto;

namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoCompletarRequest
    {
        public MetodoPagamento MetodoPagamento { get; set; }
        public List<ProdutoVendaRequest>? Produtos { get; set; }
    }
}
