using BarbeariaRocha.Modelos.Request.Excecao;
using System.Diagnostics.CodeAnalysis;

namespace BarbeariaRocha.Modelos.Entidades
{
    public class Excecao
    {
        public int Id { get; set; }
        public required DateTime Data { get; set; }
        public required string Descricao { get; set; }
        public int? BarbeiroId { get; set; }
        public int Excluido { get; set; }

        public Excecao() { }

        [SetsRequiredMembers]
        public Excecao(ExcecaoCriarRequest request)
        {
            ValidarData(request.Data);
            ValidarDescricao(request.Descricao);

            Data = request.Data;
            Descricao = request.Descricao;
            BarbeiroId = request.BarbeiroId;
            Excluido = 0;
        }

        private static void ValidarData(DateTime data)
        {
            if (data.Date < DateTime.Now.Date)
                throw new ArgumentException("A data da exceção não pode ser no passado.", nameof(data));
        }

        private static void ValidarDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição da exceção é obrigatória.", nameof(descricao));
        }
    }
}
