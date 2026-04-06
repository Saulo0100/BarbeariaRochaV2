# BarbeariaRocha API

API REST para gerenciamento de barbearias. Suporta múltiplos estabelecimentos (multi-tenant) em um único banco de dados, isolados por `TenantId`.

## Stack

- **Runtime:** .NET 10.0 / ASP.NET Core
- **Banco:** PostgreSQL (Neon Cloud) via EF Core 10 + Npgsql
- **Auth:** JWT Bearer (HMAC-SHA256, 12h)
- **Logs:** Serilog (console estruturado)
- **Deploy:** Docker + Traefik + Let's Encrypt

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL acessível (local ou Neon/cloud)

## Setup local

### 1. Configurar variáveis de ambiente

```bash
cp .env.example .env
# edite .env com seus valores reais
```

Ou configure diretamente em `appsettings.Development.json` (nunca commite secrets).

### 2. Aplicar migrations

```bash
dotnet ef database update
```

### 3. Rodar

```bash
dotnet run
```

A API estará disponível em `https://localhost:44391`.  
Swagger: `https://localhost:44391/swagger`  
Health check: `https://localhost:44391/health`

## Multi-tenancy

Cada request deve incluir o header `Origin` com o domínio cadastrado em `TenantDominio`.  
Em desenvolvimento, `https://localhost:44396` é permitido sem validação.

Novo tenant:
```
POST /api/v1/tenant
{ "nome": "Barbearia X", "planoId": 1, "dominio": "https://barbearix.com.br" }
```

## Autenticação

```
POST /api/v1/autenticacao/login
{ "numero": "11999998888", "senha": "senha123" }
```

Retorna um JWT. Use como `Authorization: Bearer <token>` nas demais requisições.

## Rotas principais (v1)

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/v1/autenticacao/login` | Login |
| POST | `/api/v1/autenticacao/redefinirsenha` | Redefinir senha |
| GET | `/api/v1/agendamento` | Listar agendamentos |
| POST | `/api/v1/agendamento` | Criar agendamento |
| GET | `/api/v1/horario/disponiveis` | Horários disponíveis |
| GET | `/api/v1/servico` | Listar serviços |
| GET | `/api/v1/relatorio/geral` | Relatório geral |
| GET | `/api/v1/configuracao-horario` | Configuração de horários |
| GET | `/health` | Health check |

## Padrões de código

- **Nomenclatura:** Português (entidades, serviços, variáveis)
- **Soft delete:** campo `Excluido = true` — nunca hard-delete
- **TenantId:** obrigatório em todas as entidades — use `ITenantService.ObterTenantId()`
- **Repository pattern:** novos serviços devem usar `IRepositorio<T>` em vez de `Contexto` diretamente
- **Sem dados sensíveis:** nunca commite `appsettings.json` com secrets reais — use variáveis de ambiente

## Docker

```bash
docker compose up -d
```

Requer `traefik-proxy` network externa e variáveis de ambiente configuradas no host.

## Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar ao banco
dotnet ef database update
```
