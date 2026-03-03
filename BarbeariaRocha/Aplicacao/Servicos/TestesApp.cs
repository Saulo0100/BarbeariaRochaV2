using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class TestesApp(Contexto contexto) : ITestesApp
    {
        private readonly Contexto _contexto = contexto;
        public void LimparEPopular()
        {
            LimparBanco();
            PopularDadosIniciais();
        }

        public void LimparBanco()
        {
            _contexto.Agendamento.RemoveRange(_contexto.Agendamento);
            _contexto.CodigoConfirmacao.RemoveRange(_contexto.CodigoConfirmacao);
            _contexto.Excecao.RemoveRange(_contexto.Excecao);
            _contexto.Mensalista.RemoveRange(_contexto.Mensalista);
            _contexto.Servico.RemoveRange(_contexto.Servico);
            _contexto.Usuario.RemoveRange(_contexto.Usuario);
            _contexto.SaveChanges();
        }

        public void PopularDadosIniciais()
        {
            // Usuários
            var usuario1 = new Usuario
            {
                Nome = "Saulo gustavo",
                Numero = "11987654321",
                Email = "cliente1@teste.com",
                Perfil = Perfil.Cliente.ToString(),
                Senha = "senha123"
            };
            var usuario2 = new Usuario
            {
                Nome = "Gerson Lucas",
                Numero = "11912345678",
                Email = "barbeiro1@teste.com",
                Perfil = Perfil.Barbeiro.ToString(),
                Senha = "senha123",
                Agenda = TipoAgenda.Semanal.ToString()
            };
            var usuario3 = new Usuario
            {
                Nome = "Yago Rocha",
                Numero = "11999999999",
                Email = "admin@teste.com",
                Perfil = Perfil.BarbeiroAdministrador.ToString(),
                Senha = "senha123",
                Agenda = TipoAgenda.Mensal.ToString()
            };

            var usuario4 = new Usuario
            {
                Nome = "Neymar Junior",
                Numero = "11999999999",
                Email = "admin@teste.com",
                Perfil = Perfil.Barbeiro.ToString(),
                Senha = "senha123",
                Agenda = TipoAgenda.Mensal.ToString()
            };

            var usuario5 = new Usuario
            {
                Nome = "Vinicius Junior",
                Numero = "11999999999",
                Email = "admin@teste.com",
                Perfil = Perfil.Barbeiro.ToString(),
                Senha = "senha123",
                Agenda = TipoAgenda.Mensal.ToString()
            };

            _contexto.Usuario.AddRange(usuario1, usuario2, usuario3);
            _contexto.SaveChanges();

            // Serviços
            var servico1 = new Servico
            {
                Descricao = "Social",
                Valor = 30.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico2 = new Servico
            {
                Descricao = "Degrade",
                Valor = 35.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico3 = new Servico
            {
                Descricao = "Navalhado ou Shaver",
                Valor = 40.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico4 = new Servico
            {
                Descricao = "Cabelo e Barba",
                Valor = 60.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico5 = new Servico
            {
                Descricao = "Barba",
                Valor = 30.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Barba"
            };

            var servico6 = new Servico
            {
                Descricao = "Sobrancelha",
                Valor = 5.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Sombrancelha"
            };

            var servico7 = new Servico
            {
                Descricao = "Luzes com Corte",
                Valor = 150.00m,
                TempoEstimado = new TimeOnly(1, 20, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico8 = new Servico
            {
                Descricao = "Luzes sem Corte",
                Valor = 120.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico9 = new Servico
            {
                Descricao = "Platinado com Corte",
                Valor = 150.00m,
                TempoEstimado = new TimeOnly(1, 20, 0),
                Excluido = false,
                Categoria = "Cabelo",
                RequerDuasEtapas = true,
                IntervaloMinimoHoras = 4,
                DescricaoEtapa1 = "Platinado",
                DescricaoEtapa2 = "Corte"
            };

            var servico10 = new Servico
            {
                Descricao = "Platinado sem Corte",
                Valor = 120.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico11 = new Servico
            {
                Descricao = "Alisamento Americano",
                Valor = 25.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            var servico12 = new Servico
            {
                Descricao = "Hidratação",
                Valor = 20.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };

            //var servico13 = new Servico
            //{
            //    Descricao = "Progressiva com Corte",
            //    Valor = 100.00m,
            //    TempoEstimado = new TimeOnly(0, 60, 0),
            //    Excluido = false,
            //    Categoria = "Cabelo"
            //};

            //var servico14 = new Servico
            //{
            //    Descricao = "Progressiva sem Corte",
            //    Valor = 70.00m,
            //    TempoEstimado = new TimeOnly(0, 60, 0),
            //    Excluido = false,
            //    Categoria = "Cabelo"
            //};
            _contexto.Servico.AddRange(servico1, servico2, servico3, servico4, servico5, servico6, servico7, servico8, servico9, servico10, servico11, servico12);
            _contexto.SaveChanges();

            var amanha = DateTime.Today.AddDays(1);

            var horarios = new List<DateTime>
            {
                new DateTime(amanha.Year, amanha.Month, amanha.Day, 16, 00, 0),
                new DateTime(amanha.Year, amanha.Month, amanha.Day, 10, 0, 0),
                new DateTime(amanha.Year, amanha.Month, amanha.Day, 10, 40, 0)
            };
            // Agendamentos
            var agendamento1 = new Agendamento
            {
                UsuarioId = usuario1.Id,
                BarbeiroId = usuario2.Id,
                ServicoId = servico1.Id,
                NomeCliente = usuario1.Nome,
                NumeroCliente = usuario1.Numero,
                Status = AgendamentoStatus.Confirmado.ToString(),
                DataHora = DateTime.SpecifyKind(horarios.FirstOrDefault(), DateTimeKind.Unspecified),
                MetodoPagamento = "Dinheiro"
            };
            var agendamento2 = new Agendamento
            {
                UsuarioId = null,
                BarbeiroId = usuario2.Id,
                ServicoId = servico2.Id,
                NomeCliente = "Cliente Avulso",
                NumeroCliente = "11977777777",
                Status = AgendamentoStatus.Pendente.ToString(),
                DataHora = DateTime.SpecifyKind(horarios[1], DateTimeKind.Unspecified),
                MetodoPagamento = null
            };

            var agendamento3 = new Agendamento
            {
                UsuarioId = null,
                BarbeiroId = usuario2.Id,
                ServicoId = servico2.Id,
                NomeCliente = "Cliente Avulso",
                NumeroCliente = "11977777777",
                Status = AgendamentoStatus.Pendente.ToString(),
                DataHora = DateTime.SpecifyKind(horarios[2], DateTimeKind.Unspecified),
                MetodoPagamento = null
            };
            _contexto.Agendamento.AddRange(agendamento1, agendamento2);
            _contexto.SaveChanges();

            // Mensalistas
            var mensalista1 = new Mensalista
            {
                Nome = "Mensalista Teste",
                Numero = "11966666666",
                Valor = 100.00m,
                Dia = "Segunda",
                Tipo = MensalistaTipo.Mensal.ToString(),
                Status = MensalistaStatus.Ativo.ToString(),
                Excluido = false
            };
            _contexto.Mensalista.Add(mensalista1);
            _contexto.SaveChanges();

            // Exceções
            var excecao1 = new Excecao
            {
                Data = DateTime.SpecifyKind(DateTime.Now.AddDays(2), DateTimeKind.Unspecified),
                Descricao = "Feriado",
                BarbeiroId = usuario2.Id,
                Excluido = false
            };
            _contexto.Excecao.Add(excecao1);
            _contexto.SaveChanges();
        }
    }
}
