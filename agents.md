# Barbearia Rocha - Backend (agents.md)

## Visao Geral
Sistema de gestao para barbearia com agendamento online, relatorios detalhados para barbeiros e painel administrativo. Backend em C# .NET 10 com Entity Framework Core e PostgreSQL.

## Regras de Negocio

### Horarios de Funcionamento
- **Segunda a Sexta**: 09:00 as 20:00
- **Sabado**: 10:00 as 20:00
- **Domingo**: Fechado (nao abre)
- **Intervalo de almoco (Seg-Sex)**: 12:20 - 13:20
- **Intervalo de almoco (Sabado)**: 12:00 - 13:00
- **Duracao do slot**: 40 minutos

**Configuracao**: Os horarios sao configurados em `Aplicacao/Helper/HelperGenerico.cs` atraves de constantes facilmente modificaveis no topo da classe.

### Slots de Horario
Os slots sao gerados automaticamente:
- **Seg-Sex**: 09:00, 09:40, 10:20, 11:00, 11:40, 12:20 | 13:20, 14:00, 14:40, 15:20, 16:00, 16:40, 17:20, 18:00, 18:40, 19:20, 20:00
- **Sabado**: 10:00, 10:40, 11:20, 12:00 | 13:00, 13:40, 14:20, 15:00, 15:40, 16:20, 17:00, 17:40, 18:20, 19:00, 19:40

### Agendamento
- Agendamento pode ser feito **sem login** (anonimo) ou com login
- Requer: barbeiro, servico, data/hora, nome e numero do cliente
- Confirmacao via codigo WhatsApp (4 digitos)
- Status possiveis: Pendente, Confirmado, LembreteEnviado, Concluido, CanceladoPeloCliente, CanceladoPeloBarbeiro, VouAtrasar, SlotReservado
- Excecoes (dias bloqueados) podem ser cadastradas por barbeiro ou globalmente

### Perfis de Usuario
- **Cliente**: Pode agendar e ver seus agendamentos
- **Barbeiro**: Pode ver corte atual, agenda, completar/cancelar agendamentos
- **BarbeiroAdministrador**: Barbeiro + funcoes administrativas
- **Administrador**: Acesso total ao sistema

### Relatorios (BI do Barbeiro)
Endpoints disponiveis em `/api/relatorio/`:
- **geral**: Total de cortes, faturamento (hoje/semana/mes/total), ticket medio, cancelamentos
- **servicos-mais-pedidos**: Ranking de servicos por quantidade, com valor e percentual
- **clientes-frequentes**: Ranking de clientes por frequencia, com gasto total
- **faturamento-diario**: Faturamento dia a dia (padrao: ultimos 30 dias)
- **faturamento-por-metodo**: Distribuicao por metodo de pagamento (PIX, Dinheiro, etc)
- **por-barbeiro**: Comparativo entre barbeiros (cortes, faturamento, ticket medio)

Todos os endpoints de relatorio aceitam filtros opcionais: `BarbeiroId`, `DataInicio`, `DataFim`.

## Arquitetura

### Estrutura de Pastas
```
BarbeariaRocha/
├── Aplicacao/
│   ├── Contratos/          # Interfaces (IAgendamentoApp, IRelatorioApp, IHorarioApp, etc)
│   ├── Helper/             # Utilitarios (HelperGenerico, Criptografia)
│   └── Servicos/           # Implementacoes (AgendamentoApp, RelatorioApp, HorarioApp, etc)
├── Configurations/         # Swagger config
├── Controllers/            # API Controllers
├── Infraestrutura/
│   ├── Contexto/           # DbContext (EF Core)
│   ├── Middlewares/        # Exception middleware
│   └── TokenProvider.cs    # JWT token generation
├── Migrations/             # EF Core migrations
├── Modelos/
│   ├── Entidades/          # Database entities
│   ├── Enums/              # AgendamentoStatus, Perfil, TipoAgenda, etc
│   ├── Paginacao/          # PaginacaoFiltro, PaginacaoResultado
│   ├── Request/            # DTOs de entrada
│   ├── Response/           # DTOs de saida
│   └── Whatsapp/           # Modelo de envio WhatsApp
└── Program.cs              # Entry point, DI, pipeline
```

### Endpoints da API
| Rota | Metodo | Auth | Descricao |
|------|--------|------|-----------|
| `POST /api/agendamento` | POST | Nao | Criar agendamento (com ou sem login) |
| `GET /api/agendamento` | GET | Sim | Listar agendamentos (paginado) |
| `GET /api/agendamento/{id}` | GET | Sim | Detalhes de um agendamento |
| `GET /api/agendamento/Atual` | GET | Sim | Corte atual do barbeiro |
| `GET /api/agendamento/HorariosOcupados` | GET | Nao | Horarios ocupados |
| `PUT /api/agendamento/{id}/EditarEcompletar` | PUT | Sim | Editar e completar |
| `PATCH /api/agendamento/{id}/Completar` | PATCH | Sim | Marcar como concluido |
| `DELETE /api/agendamento/{id}` | DELETE | Sim | Cancelar agendamento |
| `GET /api/horario/disponiveis` | GET | Nao | Horarios disponiveis para barbeiro+data |
| `GET /api/horario/todos` | GET | Nao | Todos os slots de uma data |
| `GET /api/relatorio/geral` | GET | Sim | Relatorio geral (cortes, faturamento) |
| `GET /api/relatorio/servicos-mais-pedidos` | GET | Sim | Servicos mais pedidos |
| `GET /api/relatorio/clientes-frequentes` | GET | Sim | Clientes mais frequentes |
| `GET /api/relatorio/faturamento-diario` | GET | Sim | Faturamento por dia |
| `GET /api/relatorio/faturamento-por-metodo` | GET | Sim | Faturamento por metodo pgto |
| `GET /api/relatorio/por-barbeiro` | GET | Sim | Comparativo entre barbeiros |
| `POST /api/autenticacao/login` | POST | Nao | Login |
| `POST /api/autenticacao/cadastro` | POST | Nao | Cadastro de usuario |
| `GET /api/usuario` | GET | Sim | Listar usuarios |
| `GET /api/servico` | GET | Nao | Listar servicos |
| `POST /api/token/gerar` | POST | Nao | Gerar codigo confirmacao |
| `POST /api/token/validar` | POST | Nao | Validar codigo confirmacao |

### Banco de Dados
- **PostgreSQL** (hospedado no Neon)
- **ORM**: Entity Framework Core
- **Padrao**: Soft delete (campo `Excluido` nas entidades)
- **Entidades**: Usuario, Agendamento, Servico, CodigoConfirmacao, Excecao, Mensalista

### Autenticacao
- **JWT Bearer Token**
- Claims no token: Id, Nome, Numero, Perfil
- Configuracao em `appsettings.json` (Jwt:Secret, Jwt:Issuer, Jwt:Audience)

### Como Alterar Horarios
Para modificar os horarios de funcionamento, edite as constantes no topo de `HelperGenerico.cs`:
```csharp
private const int IntervaloMinutos = 40;        // Duracao do slot
private static readonly TimeOnly SemanaInicio = TimeOnly.Parse("09:00");  // Inicio seg-sex
private static readonly TimeOnly SemanaFim = TimeOnly.Parse("20:00");     // Fim seg-sex
private static readonly TimeOnly SabadoInicio = TimeOnly.Parse("10:00");  // Inicio sabado
private static readonly TimeOnly SabadoFim = TimeOnly.Parse("20:00");     // Fim sabado
```
