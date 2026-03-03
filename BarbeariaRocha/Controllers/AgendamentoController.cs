using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Agendamento;
using BarbeariaRocha.Modelos.Response.Agendamento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/agendamento")]
    public class AgendamentoController(IAgendamentoApp app) : BaseController
    {
        private readonly IAgendamentoApp _app = app;

        // GET: api/agendamento/horarios
        [HttpGet("HorariosOcupados")]
        public ActionResult<List<HorariosOcupadosResponse>> ObterHorariosOcupados([FromQuery] HorarioRequest request)
        {
            var horarios = _app.HorariosOcupadosBarbeiro(request);
            return Ok(horarios);
        }

        // POST: api/agendamento
        // Permite agendamento sem login (usuario anonimo)
        [HttpPost]
        public IActionResult Criar([FromBody] AgendamentoCriarRequest request)
        {
            try
            {
                request.UsuarioId = UserId();
            }
            catch { }
            _app.CriarAgendamento(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        // GET: api/agendamento/{id}
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<AgendamentoDetalheResponse> ObterPorId(int id)
        {
            var agendamento = _app.ObterPorId(id);
            return Ok(agendamento);
        }

        // GET: api/agendamento/Atual
        [Authorize]
        [HttpGet("Atual")]
        public ActionResult<AgendamentoDetalheResponse> ObterAtual()
        {
            var agendamento = _app.AgendamentoAtual(UserId());
            return Ok(agendamento);
        }

        // PUT: api/agendamento/{id}/EditarEcompletar
        [Authorize]
        [HttpPut("{id}/EditarEcompletar")]
        public IActionResult Atualizar(int id, [FromBody] AgendamentoEditarRequest request)
        {
            _app.EditarECompletarAgendamento(id, request);
            return NoContent();
        }

        // PATCH: api/agendamento/{id}/Completar
        [Authorize]
        [HttpPatch("{id}/Completar")]
        public IActionResult Completar(int id, [FromBody] AgendamentoCompletarRequest request)
        {
            _app.CompletarAgendamento(id, request);
            return NoContent();
        }

        // POST/PATCH: api/agendamento/{id}/ClienteFaltou
        [Authorize]
        [HttpPost("{id}/ClienteFaltou")]
        public IActionResult MarcarClienteFaltou(int id)
        {
            _app.MarcarClienteFaltou(id);
            return NoContent();
        }

        // DELETE: api/agendamento/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Cancelar(int id)
        {
            _app.CancelarAgendamento(id);
            return NoContent();
        }

        // GET: api/agendamento
        [Authorize]
        [HttpGet]
        public ActionResult<PaginacaoResultado<AgendamentoDetalheResponse>> Listar([FromQuery] PaginacaoFiltro<AgendamentoFiltroRequest> filtro)
        {
            filtro.Filtro ??= new AgendamentoFiltroRequest();

            var perfil = PerfilUsuario();
            if (perfil == "Cliente")
            {
                // Cliente logado vê apenas seus próprios agendamentos
                filtro.Filtro.UsuarioId = UserId();
            }
            else if (filtro.Filtro.BarbeiroId == null)
            {
                // Barbeiro/Admin sem filtro específico vê seus próprios agendamentos
                filtro.Filtro.BarbeiroId = UserId();
            }

            var resultado = _app.ListarAgendamentos(filtro);
            return Ok(resultado);
        }
    }
}
