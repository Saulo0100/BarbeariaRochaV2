using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int UserId()
        {
            var id = User.Claims?.FirstOrDefault(a => a.Type == "Id")?.Value;
            return id == null ? throw new Exception("Não autorizado") : int.Parse(id);
        }
        protected string Numero()
        {
            var numero = User.Claims?.FirstOrDefault(a => a.Type == "Numero")?.Value;
            return numero ?? throw new Exception("Não autorizado");
        }
        protected string PerfilUsuario()
        {
            var perfil = User.Claims?.FirstOrDefault(a => a.Type == "Perfil")?.Value;
            return perfil ?? throw new Exception("Não autorizado");
        }
    }
}
