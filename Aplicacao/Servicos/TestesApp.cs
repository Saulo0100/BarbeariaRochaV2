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
                Nome = "Yago Rocha",
                Numero = "41998254308",
                Email = "cliente1@teste.com",
                Perfil = Perfil.BarbeiroAdministrador.ToString(),
                Senha = "senhasegura123"
            };

            _contexto.Usuario.Add(usuario1);
            _contexto.SaveChanges();

            
            var servico1 = new Servico
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
            _contexto.Servico.Add(servico1);
            _contexto.SaveChanges();
        }
    }
}
