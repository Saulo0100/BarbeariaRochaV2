using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class TestesApp(Contexto contexto) : ITestesApp
    {
        private readonly Contexto _contexto = contexto;
        public void RecriarBanco()
        {
            throw new NotImplementedException();
        }
    }
}
