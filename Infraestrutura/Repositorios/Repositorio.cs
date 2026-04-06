using Microsoft.EntityFrameworkCore;
using AppContexto = BarbeariaRocha.Infraestrutura.Contexto.Contexto;

namespace BarbeariaRocha.Infraestrutura.Repositorios;

public class Repositorio<T>(AppContexto contexto) : IRepositorio<T> where T : class
{
    private readonly DbSet<T> _dbSet = contexto.Set<T>();

    public IQueryable<T> Query() => _dbSet.AsQueryable();

    public async Task<T?> ObterPorIdAsync(int id) =>
        await _dbSet.FindAsync(id);

    public async Task AdicionarAsync(T entidade) =>
        await _dbSet.AddAsync(entidade);

    public void Atualizar(T entidade) =>
        _dbSet.Update(entidade);

    public void Remover(T entidade) =>
        _dbSet.Remove(entidade);

    public async Task SalvarAsync() =>
        await contexto.SaveChangesAsync();
}
