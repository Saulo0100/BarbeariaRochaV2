using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Agendamento;
using BarbeariaRocha.Modelos.Response.Agendamento;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IAgendamentoApp
    {
        // Horários disponíveis de um barbeiro
        List<HorariosOcupadosResponse> HorariosOcupadosBarbeiro(HorarioRequest request);

        // Criar novo agendamento
        void CriarAgendamento(AgendamentoCriarRequest request);

        // Buscar agendamento por Id
        AgendamentoDetalheResponse ObterPorId(int id);

        // Buscar agendamento atual do usuário logado
        AgendamentoDetalheResponse AgendamentoAtual(int usuarioId);

        // Atualizar agendamento
        void EditarECompletarAgendamento(int id, AgendamentoEditarRequest request);

        // Listar agendamentos com paginação
        PaginacaoResultado<AgendamentoDetalheResponse> ListarAgendamentos(PaginacaoFiltro<AgendamentoFiltroRequest> request);

        // Completar agendamento
        void CompletarAgendamento(AgendamentoCompletarRequest request);

        // Cancelar agendamento
        void CancelarAgendamento(int id);
    }
}
