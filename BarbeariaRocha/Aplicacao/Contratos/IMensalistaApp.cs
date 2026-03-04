using BarbeariaRocha.Modelos.Request.Mensalista;
using BarbeariaRocha.Modelos.Response.Mensalista;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IMensalistaApp
    {
        void CadastrarMensalista(MensalistaCriarRequest request, int idUsuario);
        void CancelarMensalista(int idMensalista);
        IEnumerable<MensalistaResponse> ObterTodos();
        void RegistrarCorte(MensalistaRegistrarCorteRequest request);
        IEnumerable<MensalistaCorteResponse> ListarCortes(int mensalistaId, int? mes = null, int? ano = null);
        void DeletarCorte(int corteId);
    }
}
