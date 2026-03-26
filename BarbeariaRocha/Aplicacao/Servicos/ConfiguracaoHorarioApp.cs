using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Request.Horario;
using BarbeariaRocha.Modelos.Response.Horario;
using System.Globalization;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ConfiguracaoHorarioApp(Contexto contexto) : IConfiguracaoHorarioApp
    {
        private readonly Contexto _contexto = contexto;

        private static readonly string[] NomesDias = ["Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado"];

        public List<ConfiguracaoHorarioResponse> ListarTodos()
        {
            return _contexto.ConfiguracaoHorario
                .OrderBy(c => c.DiaSemana)
                .ToList()
                .Select(MapearResponse)
                .ToList();
        }

        public ConfiguracaoHorarioResponse ObterPorDiaSemana(int diaSemana)
        {
            var config = _contexto.ConfiguracaoHorario
                .FirstOrDefault(c => c.DiaSemana == diaSemana)
                ?? throw new Exception($"Configuração para o dia {diaSemana} não encontrada.");

            return MapearResponse(config);
        }

        public void Salvar(ConfiguracaoHorarioSalvarRequest request)
        {
            ValidarRequest(request);

            var config = _contexto.ConfiguracaoHorario
                .FirstOrDefault(c => c.DiaSemana == request.DiaSemana);

            if (config == null)
            {
                config = new ConfiguracaoHorario { DiaSemana = request.DiaSemana };
                _contexto.ConfiguracaoHorario.Add(config);
            }

            AplicarRequest(config, request);
            _contexto.SaveChanges();
        }

        public void SalvarTodos(List<ConfiguracaoHorarioSalvarRequest> requests)
        {
            foreach (var request in requests)
                ValidarRequest(request);

            foreach (var request in requests)
            {
                var config = _contexto.ConfiguracaoHorario
                    .FirstOrDefault(c => c.DiaSemana == request.DiaSemana);

                if (config == null)
                {
                    config = new ConfiguracaoHorario { DiaSemana = request.DiaSemana };
                    _contexto.ConfiguracaoHorario.Add(config);
                }

                AplicarRequest(config, request);
            }

            _contexto.SaveChanges();
        }

        private static void ValidarRequest(ConfiguracaoHorarioSalvarRequest request)
        {
            if (request.DiaSemana < 0 || request.DiaSemana > 6)
                throw new Exception("DiaSemana deve ser entre 0 (Domingo) e 6 (Sábado).");

            if (request.IntervaloMinutos <= 0)
                throw new Exception("IntervaloMinutos deve ser maior que zero.");

            if (request.Aberto)
            {
                if (string.IsNullOrWhiteSpace(request.HoraInicio))
                    throw new Exception("HoraInicio é obrigatório quando o dia está aberto.");
                if (string.IsNullOrWhiteSpace(request.HoraFim))
                    throw new Exception("HoraFim é obrigatório quando o dia está aberto.");
                if (!string.IsNullOrWhiteSpace(request.AlmocoInicio) && string.IsNullOrWhiteSpace(request.AlmocoFim))
                    throw new Exception("AlmocoFim é obrigatório quando AlmocoInicio é informado.");
                if (!string.IsNullOrWhiteSpace(request.AlmocoFim) && string.IsNullOrWhiteSpace(request.AlmocoInicio))
                    throw new Exception("AlmocoInicio é obrigatório quando AlmocoFim é informado.");
            }
        }

        private static void AplicarRequest(ConfiguracaoHorario config, ConfiguracaoHorarioSalvarRequest request)
        {
            config.Aberto = request.Aberto;
            config.IntervaloMinutos = request.IntervaloMinutos;
            config.HoraInicio = ParseHora(request.HoraInicio);
            config.AlmocoInicio = ParseHora(request.AlmocoInicio);
            config.AlmocoFim = ParseHora(request.AlmocoFim);
            config.HoraFim = ParseHora(request.HoraFim);
        }

        private static TimeOnly? ParseHora(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            return TimeOnly.Parse(valor);
        }

        private static ConfiguracaoHorarioResponse MapearResponse(ConfiguracaoHorario c) => new()
        {
            Id = c.Id,
            DiaSemana = c.DiaSemana,
            NomeDia = NomesDias[c.DiaSemana],
            Aberto = c.Aberto,
            HoraInicio = c.HoraInicio?.ToString("HH:mm"),
            AlmocoInicio = c.AlmocoInicio?.ToString("HH:mm"),
            AlmocoFim = c.AlmocoFim?.ToString("HH:mm"),
            HoraFim = c.HoraFim?.ToString("HH:mm"),
            IntervaloMinutos = c.IntervaloMinutos
        };
    }
}
