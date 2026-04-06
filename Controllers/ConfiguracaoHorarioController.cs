using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Request.Horario;
using BarbeariaRocha.Modelos.Response.Horario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/v1/configuracao-horario")]
    public class ConfiguracaoHorarioController(IConfiguracaoHorarioApp app) : ControllerBase
    {
        private readonly IConfiguracaoHorarioApp _app = app;

        /// <summary>
        /// Retorna a configuração de horários de todos os dias da semana.
        /// Endpoint público — usado pelo frontend para exibir o horário de funcionamento.
        /// </summary>
        [HttpGet]
        public ActionResult<List<ConfiguracaoHorarioResponse>> ListarTodos()
        {
            var resultado = _app.ListarTodos();
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna a configuração de horário de um dia específico.
        /// 0=Domingo, 1=Segunda, ..., 6=Sábado.
        /// </summary>
        [HttpGet("{diaSemana:int}")]
        public ActionResult<ConfiguracaoHorarioResponse> ObterPorDia(int diaSemana)
        {
            var resultado = _app.ObterPorDiaSemana(diaSemana);
            return Ok(resultado);
        }

        /// <summary>
        /// Atualiza a configuração de horário de um único dia da semana.
        /// Requer autenticação de administrador.
        /// </summary>
        [HttpPut]
        [Authorize]
        public IActionResult Salvar([FromBody] ConfiguracaoHorarioSalvarRequest request)
        {
            _app.Salvar(request);
            return NoContent();
        }

        /// <summary>
        /// Atualiza a configuração de horários de todos os dias de uma vez.
        /// Requer autenticação de administrador.
        /// </summary>
        [HttpPut("todos")]
        [Authorize]
        public IActionResult SalvarTodos([FromBody] List<ConfiguracaoHorarioSalvarRequest> requests)
        {
            _app.SalvarTodos(requests);
            return NoContent();
        }
    }
}
