using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Excecoes;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Infraestrutura.Repositorios;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Request.Produto;
using BarbeariaRocha.Modelos.Response.Produto;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ProdutoApp(
        IRepositorio<Produto> produtoRepo,
        IRepositorio<MovimentacaoEstoque> movimentacaoRepo,
        IRepositorio<AgendamentoProduto> agendamentoProdutoRepo,
        ITenantService tenantService) : IProdutoApp
    {
        public List<ProdutoDetalhesResponse> Listar()
        {
            var tenantId = tenantService.ObterTenantId();
            return produtoRepo.Query()
                .Where(p => p.TenantId == tenantId && !p.Excluido)
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoDetalhesResponse
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    QuantidadeEstoque = p.QuantidadeEstoque,
                    QuantidadeMinima = p.QuantidadeMinima,
                    EstoqueBaixo = p.QuantidadeEstoque <= p.QuantidadeMinima
                })
                .ToList();
        }

        public List<ProdutoDetalhesResponse> ListarPublico()
        {
            var tenantId = tenantService.ObterTenantId();
            return produtoRepo.Query()
                .Where(p => p.TenantId == tenantId && !p.Excluido && p.QuantidadeEstoque > 0)
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoDetalhesResponse
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    QuantidadeEstoque = p.QuantidadeEstoque,
                    QuantidadeMinima = p.QuantidadeMinima,
                    EstoqueBaixo = false
                })
                .ToList();
        }

        public ProdutoDetalhesResponse Criar(ProdutoCriarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new AppException("O nome do produto é obrigatório.");
            if (request.Preco <= 0)
                throw new AppException("O preço deve ser maior que zero.");
            if (request.QuantidadeInicial < 0)
                throw new AppException("A quantidade inicial não pode ser negativa.");
            if (request.QuantidadeMinima < 0)
                throw new AppException("A quantidade mínima não pode ser negativa.");

            var tenantId = tenantService.ObterTenantId();

            var duplicado = produtoRepo.Query()
                .Any(p => p.TenantId == tenantId && p.Nome.ToLower() == request.Nome.ToLower().Trim() && !p.Excluido);
            if (duplicado)
                throw new AppException("Já existe um produto com este nome.");

            var produto = new Produto
            {
                TenantId = tenantId,
                Nome = request.Nome.Trim(),
                Descricao = request.Descricao?.Trim(),
                Preco = request.Preco,
                QuantidadeEstoque = request.QuantidadeInicial,
                QuantidadeMinima = request.QuantidadeMinima
            };

            produtoRepo.AdicionarAsync(produto).GetAwaiter().GetResult();
            produtoRepo.SalvarAsync().GetAwaiter().GetResult();

            if (request.QuantidadeInicial > 0)
            {
                var movimentacao = new MovimentacaoEstoque
                {
                    TenantId = tenantId,
                    ProdutoId = produto.Id,
                    Tipo = TipoMovimentacao.Entrada,
                    Quantidade = request.QuantidadeInicial,
                    Motivo = "Estoque inicial",
                    DataMovimentacao = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };
                movimentacaoRepo.AdicionarAsync(movimentacao).GetAwaiter().GetResult();
                movimentacaoRepo.SalvarAsync().GetAwaiter().GetResult();
            }

            return new ProdutoDetalhesResponse
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                QuantidadeMinima = produto.QuantidadeMinima,
                EstoqueBaixo = produto.QuantidadeEstoque <= produto.QuantidadeMinima
            };
        }

        public void Editar(int id, ProdutoEditarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new AppException("O nome do produto é obrigatório.");
            if (request.Preco <= 0)
                throw new AppException("O preço deve ser maior que zero.");

            var tenantId = tenantService.ObterTenantId();
            var produto = produtoRepo.Query()
                .FirstOrDefault(p => p.Id == id && p.TenantId == tenantId && !p.Excluido)
                ?? throw new AppException("Produto não encontrado.");

            var duplicado = produtoRepo.Query()
                .Any(p => p.TenantId == tenantId && p.Nome.ToLower() == request.Nome.ToLower().Trim() && !p.Excluido && p.Id != id);
            if (duplicado)
                throw new AppException("Já existe um produto com este nome.");

            produto.Nome = request.Nome.Trim();
            produto.Descricao = request.Descricao?.Trim();
            produto.Preco = request.Preco;
            produto.QuantidadeMinima = request.QuantidadeMinima;

            produtoRepo.Atualizar(produto);
            produtoRepo.SalvarAsync().GetAwaiter().GetResult();
        }

        public void Excluir(int id)
        {
            var tenantId = tenantService.ObterTenantId();
            var produto = produtoRepo.Query()
                .FirstOrDefault(p => p.Id == id && p.TenantId == tenantId)
                ?? throw new AppException("Produto não encontrado.");

            produto.Excluido = true;
            produtoRepo.Atualizar(produto);
            produtoRepo.SalvarAsync().GetAwaiter().GetResult();
        }

        public void RegistrarMovimentacao(int produtoId, MovimentacaoCriarRequest request)
        {
            if (request.Quantidade <= 0)
                throw new AppException("A quantidade deve ser maior que zero.");
            if (string.IsNullOrWhiteSpace(request.Motivo))
                throw new AppException("O motivo é obrigatório.");

            var tenantId = tenantService.ObterTenantId();
            var produto = produtoRepo.Query()
                .FirstOrDefault(p => p.Id == produtoId && p.TenantId == tenantId && !p.Excluido)
                ?? throw new AppException("Produto não encontrado.");

            if (request.Tipo == TipoMovimentacao.Saida && produto.QuantidadeEstoque < request.Quantidade)
                throw new AppException($"Estoque insuficiente. Disponível: {produto.QuantidadeEstoque}.");

            produto.QuantidadeEstoque = request.Tipo == TipoMovimentacao.Entrada
                ? produto.QuantidadeEstoque + request.Quantidade
                : produto.QuantidadeEstoque - request.Quantidade;

            var movimentacao = new MovimentacaoEstoque
            {
                TenantId = tenantId,
                ProdutoId = produtoId,
                Tipo = request.Tipo,
                Quantidade = request.Quantidade,
                Motivo = request.Motivo.Trim(),
                DataMovimentacao = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            produtoRepo.Atualizar(produto);
            produtoRepo.SalvarAsync().GetAwaiter().GetResult();

            movimentacaoRepo.AdicionarAsync(movimentacao).GetAwaiter().GetResult();
            movimentacaoRepo.SalvarAsync().GetAwaiter().GetResult();
        }

        public List<MovimentacaoEstoqueResponse> ObterHistorico(int produtoId)
        {
            var tenantId = tenantService.ObterTenantId();
            return movimentacaoRepo.Query()
                .Where(m => m.ProdutoId == produtoId && m.TenantId == tenantId)
                .OrderByDescending(m => m.DataMovimentacao)
                .Select(m => new MovimentacaoEstoqueResponse
                {
                    Id = m.Id,
                    Tipo = m.Tipo.ToString(),
                    Quantidade = m.Quantidade,
                    Motivo = m.Motivo,
                    DataMovimentacao = m.DataMovimentacao,
                    AgendamentoId = m.AgendamentoId
                })
                .ToList();
        }

        public int ContarEstoqueBaixo()
        {
            var tenantId = tenantService.ObterTenantId();
            return produtoRepo.Query()
                .Count(p => p.TenantId == tenantId && !p.Excluido && p.QuantidadeEstoque <= p.QuantidadeMinima);
        }

        public void RegistrarVendas(int agendamentoId, List<ProdutoVendaRequest> produtos)
        {
            if (produtos == null || produtos.Count == 0) return;
            var tenantId = tenantService.ObterTenantId();

            foreach (var venda in produtos)
            {
                var produto = produtoRepo.Query()
                    .FirstOrDefault(p => p.Id == venda.ProdutoId && p.TenantId == tenantId && !p.Excluido)
                    ?? throw new AppException($"Produto {venda.ProdutoId} não encontrado.");

                if (produto.QuantidadeEstoque < venda.Quantidade)
                    throw new AppException($"Estoque insuficiente para '{produto.Nome}'. Disponível: {produto.QuantidadeEstoque}.");

                produto.QuantidadeEstoque -= venda.Quantidade;

                var movimentacao = new MovimentacaoEstoque
                {
                    TenantId = tenantId,
                    ProdutoId = venda.ProdutoId,
                    Tipo = TipoMovimentacao.Saida,
                    Quantidade = venda.Quantidade,
                    Motivo = "Venda em atendimento",
                    DataMovimentacao = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    AgendamentoId = agendamentoId
                };

                var agendamentoProduto = new AgendamentoProduto
                {
                    TenantId = tenantId,
                    AgendamentoId = agendamentoId,
                    ProdutoId = venda.ProdutoId,
                    Quantidade = venda.Quantidade,
                    NomeProduto = produto.Nome,
                    PrecoProduto = produto.Preco
                };

                produtoRepo.Atualizar(produto);
                movimentacaoRepo.AdicionarAsync(movimentacao).GetAwaiter().GetResult();
                agendamentoProdutoRepo.AdicionarAsync(agendamentoProduto).GetAwaiter().GetResult();
            }

            produtoRepo.SalvarAsync().GetAwaiter().GetResult();
            movimentacaoRepo.SalvarAsync().GetAwaiter().GetResult();
            agendamentoProdutoRepo.SalvarAsync().GetAwaiter().GetResult();
        }
    }
}
