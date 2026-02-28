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
                Nome = "Cliente Teste 1",
                Numero = "11987654321",
                Email = "cliente1@teste.com",
                Perfil = Perfil.Cliente.ToString(),
                Senha = "senha123"
            };
            var usuario2 = new Usuario
            {
                Nome = "Barbeiro Teste 1",
                Numero = "11912345678",
                Email = "barbeiro1@teste.com",
                Perfil = Perfil.Barbeiro.ToString(),
                Senha = "senha123",
                Agenda = TipoAgenda.Semanal.ToString()
            };
            var usuario3 = new Usuario
            {
                Nome = "Admin Teste",
                Numero = "11999999999",
                Email = "admin@teste.com",
                Perfil = Perfil.Administrador.ToString(),
                Senha = "senha123",
                Agenda = TipoAgenda.Mensal.ToString()
            };

            _contexto.Usuario.AddRange(usuario1, usuario2, usuario3);
            _contexto.SaveChanges();

            // Serviços
            var servico1 = new Servico
            {
                Descricao = "Corte de Cabelo",
                Valor = 30.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };
            var servico2 = new Servico
            {
                Descricao = "Corte de Cabelo e Barba",
                Valor = 50.00m,
                TempoEstimado = new TimeOnly(0, 40, 0),
                Excluido = false,
                Categoria = "Cabelo"
            };
            var servico3 = new Servico
            {
                Descricao = "Barba",
                Valor = 25.00m,
                TempoEstimado = new TimeOnly(0, 20, 0),
                Excluido = false,
                Categoria = "Barba"
            };
            _contexto.Servico.AddRange(servico1, servico2);
            _contexto.SaveChanges();

            var amanha = DateTime.Today.AddDays(1);

            var horarios = new List<DateTime>
            {
                new DateTime(amanha.Year, amanha.Month, amanha.Day, 9, 20, 0),
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
