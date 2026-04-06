namespace BarbeariaRocha.Infraestrutura.Repositorios;

public interface IRepositorio<T> where T : class
{
    IQueryable<T> Query();
    Task<T?> ObterPorIdAsync(int id);
    Task AdicionarAsync(T entidade);
    void Atualizar(T entidade);
    void Remover(T entidade);
    Task SalvarAsync();
}
