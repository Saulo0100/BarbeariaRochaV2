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
