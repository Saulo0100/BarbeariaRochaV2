using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Request.Mensalista;
using BarbeariaRocha.Modelos.Response.Mensalista;
using System.Globalization;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class MensalistaApp(Contexto contexto) : IMensalistaApp
    {
        private readonly Contexto _contexto = contexto;

        public void CadastrarMensalista(MensalistaCriarRequest request, int idUsuario)
        {
            ValidarDadosMensalista(request);

            var usuario = _contexto.Usuario.Find(idUsuario);
            if (usuario != null)
            {
                request.Nome = usuario.Nome;
                request.Numero = usuario.Numero;
            }
            else
            {
                Usuario.ValidarNumero(request.Numero);
            }
            var dia = CultureInfo
                    .GetCultureInfo("pt-BR")
                    .DateTimeFormat
                    .GetDayName(request.Dia);

            var mensalista = new Mensalista
            {
                Nome = request.Nome,
                Numero = request.Numero,
                Valor = request.Valor,
                Dia = dia,
                Tipo = request.Tipo.ToString(),
                Status = MensalistaStatus.Ativo.ToString()
            };

            _contexto.Mensalista.Add(mensalista);
            _contexto.SaveChanges();
        }

        public void CancelarMensalista(int idMensalista)
        {
            var mensalista = _contexto.Mensalista.Find(idMensalista)
                ?? throw new Exception("Mensalista não encontrado.");

            _contexto.Mensalista.Remove(mensalista);
            _contexto.SaveChanges();
        }

        public IEnumerable<MensalistaResponse> ObterTodos()
        {
            var mensalistas = _contexto.Mensalista
                .Select(m => new MensalistaResponse
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Tipo = m.Tipo,
                    Status = m.Status,
                    Dia = m.Dia,
                    Valor = m.Valor
                })
                .ToList();

            return mensalistas;
        }

        private static void ValidarDadosMensalista(MensalistaCriarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("O nome do mensalista é obrigatório.", nameof(request.Nome));

            if (string.IsNullOrWhiteSpace(request.Numero))
                throw new ArgumentException("O número do mensalista é obrigatório.", nameof(request.Numero));

            if (request.Valor <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.", nameof(request.Valor));

            if (!Enum.IsDefined(typeof(DayOfWeek), request.Dia))
                throw new ArgumentException("O dia é obrigatório.", nameof(request.Dia));

            if (!Enum.IsDefined(typeof(MensalistaTipo), request.Tipo))
                throw new ArgumentException("O tipo é obrigatório.", nameof(request.Tipo));

        }
    }
}
