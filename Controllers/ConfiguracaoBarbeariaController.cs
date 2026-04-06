using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Request.ConfiguracaoBarbearia;
using BarbeariaRocha.Modelos.Response.ConfiguracaoBarbearia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/v1/configuracao-barbearia")]
    public class ConfiguracaoBarbeariaController(IConfiguracaoBarbeariaApp app) : ControllerBase
    {
        private readonly IConfiguracaoBarbeariaApp _app = app;

        /// <summary>
        /// Retorna os dados de contato e localização da barbearia.
        /// Endpoint público.
        /// </summary>
        [HttpGet]
        public ActionResult<ConfiguracaoBarbeariaResponse?> Obter()
        {
            var resultado = _app.Obter();
            return Ok(resultado);
        }

        /// <summary>
        /// Cria a configuração da barbearia. Só pode existir um registro.
        /// </summary>
        [HttpPost]
        [Authorize]
        public ActionResult<ConfiguracaoBarbeariaResponse> Criar([FromBody] ConfiguracaoBarbeariaRequest request)
        {
            var resultado = _app.Criar(request);
            return CreatedAtAction(nameof(Obter), resultado);
        }

        /// <summary>
        /// Edita a configuração da barbearia.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public ActionResult<ConfiguracaoBarbeariaResponse> Editar(int id, [FromBody] ConfiguracaoBarbeariaRequest request)
        {
            var resultado = _app.Editar(id, request);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta a configuração da barbearia.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult Deletar(int id)
        {
            _app.Deletar(id);
            return NoContent();
        }
    }
}
