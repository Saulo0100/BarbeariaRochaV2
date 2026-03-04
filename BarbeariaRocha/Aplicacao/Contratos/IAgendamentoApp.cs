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

        // Criar agendamento para cliente (barbeiro/admin cria sem código de confirmação)
        void CriarAgendamentoParaCliente(AgendamentoCriarParaClienteRequest request);

        // Buscar agendamento por Id
        AgendamentoDetalheResponse ObterPorId(int id);

        // Buscar agendamento atual do usuário logado
        AgendamentoDetalheResponse AgendamentoAtual(int usuarioId);

        // Atualizar agendamento
        void EditarECompletarAgendamento(int id, AgendamentoEditarRequest request);

        // Listar agendamentos com paginação
        PaginacaoResultado<AgendamentoDetalheResponse> ListarAgendamentos(PaginacaoFiltro<AgendamentoFiltroRequest> request);

        // Completar agendamento
        void CompletarAgendamento(int id, AgendamentoCompletarRequest request);

        // Cancelar agendamento
        void CancelarAgendamento(int id);

        // Marcar cliente como faltou (no-show)
        void MarcarClienteFaltou(int id);
    }
}
