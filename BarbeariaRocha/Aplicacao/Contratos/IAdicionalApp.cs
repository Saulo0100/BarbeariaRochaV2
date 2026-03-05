namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IAdicionalApp
    {
        List<object> ListarAdicionais();
        void CriarAdicional(string nome, decimal valor);
        void DeletarAdicional(int id);
    }
}
