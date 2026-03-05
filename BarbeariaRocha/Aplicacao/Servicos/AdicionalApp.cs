using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class AdicionalApp(Contexto contexto) : IAdicionalApp
    {
        private readonly Contexto _contexto = contexto;

        public List<object> ListarAdicionais()
        {
            return _contexto.Adicional
                .Where(a => !a.Excluido)
                .Select(a => (object)new { id = a.Id, nome = a.Nome, valor = a.Valor })
                .ToList();
        }

        public void CriarAdicional(string nome, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new Exception("O nome do adicional é obrigatório.");

            if (valor <= 0)
                throw new Exception("O valor do adicional deve ser maior que zero.");

            var duplicado = _contexto.Adicional
                .Any(a => a.Nome.ToLower() == nome.ToLower().Trim() && !a.Excluido);

            if (duplicado)
                throw new Exception("Já existe um adicional com este nome.");

            var adicional = new Adicional
            {
                Nome = nome.Trim(),
                Valor = valor
            };

            _contexto.Adicional.Add(adicional);
            _contexto.SaveChanges();
        }

        public void DeletarAdicional(int id)
        {
            var adicional = _contexto.Adicional.Find(id)
                ?? throw new Exception("Adicional não encontrado.");

            adicional.Excluido = true;
            _contexto.SaveChanges();
        }
    }
}
