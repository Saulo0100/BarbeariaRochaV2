using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Response.Horario;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/horario")]
    public class HorarioController(IHorarioApp app) : ControllerBase
    {
        private readonly IHorarioApp _app = app;

        /// <summary>
        /// Retorna horarios disponiveis e ocupados para um barbeiro em uma data.
        /// Nao requer autenticacao - usado no fluxo de agendamento publico.
        /// </summary>
        [HttpGet("disponiveis")]
        public ActionResult<HorariosDisponiveisResponse> ObterHorariosDisponiveis(
            [FromQuery] int barbeiroId,
            [FromQuery] DateTime data)
        {
            var resultado = _app.ObterHorariosDisponiveis(barbeiroId, data);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna horarios disponiveis considerando o tipo de servico.
        /// Para servicos de 1h20m: retorna apenas horarios com 2 slots consecutivos disponiveis.
        /// Para servicos com 2 etapas: retorna apenas horarios da etapa 1 que possuem horarios validos para etapa 2.
        /// </summary>
        [HttpGet("disponiveis-servico")]
        public ActionResult<HorariosDisponiveisServicoResponse> ObterHorariosDisponiveisPorServico(
            [FromQuery] int barbeiroId,
            [FromQuery] DateTime data,
            [FromQuery] int servicoId)
        {
            var resultado = _app.ObterHorariosDisponiveisPorServico(barbeiroId, data, servicoId);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna horarios disponiveis para a etapa 2 de um servico com 2 etapas,
        /// dado o horario selecionado para a etapa 1.
        /// </summary>
        [HttpGet("disponiveis-etapa2")]
        public ActionResult<HorariosDisponiveisServicoResponse> ObterHorariosEtapa2(
            [FromQuery] int barbeiroId,
            [FromQuery] DateTime data,
            [FromQuery] int servicoId,
            [FromQuery] string horaEtapa1)
        {
            var resultado = _app.ObterHorariosEtapa2(barbeiroId, data, servicoId, horaEtapa1);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna todos os horarios padrao para uma data (sem verificar ocupacao).
        /// Util para montar a grade de horarios no frontend.
        /// </summary>
        [HttpGet("todos")]
        public ActionResult<List<string>> ObterTodosHorarios([FromQuery] DateTime data)
        {
            var resultado = _app.ObterTodosHorariosPorData(data);
            return Ok(resultado);
        }
    }
}
