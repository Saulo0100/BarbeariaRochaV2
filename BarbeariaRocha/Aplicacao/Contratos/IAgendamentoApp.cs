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

        // Gerar token para cancelamento sem login
        void GerarTokenCancelamento(string numero);

        // Listar agendamentos pendentes por número (após verificação de código)
        List<AgendamentoDetalheResponse> ListarPendentesPorNumero(string numero, int codigo);

        // Cancelar agendamento sem login (com verificação de código)
        void CancelarPorNumero(int agendamentoId, string numero, int codigo);

        // Cancelar agendamento como cliente logado (sem código)
        void CancelarAgendamentoComoCliente(int agendamentoId, int clienteId);

        // Listar agendamentos pendentes do cliente logado
        List<AgendamentoDetalheResponse> ListarAgendamentosCliente(int clienteId);

        // Obter próximo agendamento por número (após verificação de código)
        AgendamentoDetalheResponse? ProximoAgendamentoPorNumero(string numero, int codigo);
    }
}
