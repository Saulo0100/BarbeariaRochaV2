using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Request.Produto;

namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoEditarRequest
    {
        public required MetodoPagamento MetodoPagamento { get; set; }
        public int ServicoId { get; set; }
        public List<AdicionalRequest>? Adicionais { get; set; }
        public List<ProdutoVendaRequest>? Produtos { get; set; }
    }
}
