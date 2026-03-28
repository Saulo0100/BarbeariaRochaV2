# Refatoração Multi-Tenant e Sistema de Planos

## Contexto
Tenho uma API em .NET com arquitetura multi-tenant baseada em domínio. Atualmente existe um TenantValidationMiddleware que faz uma requisição HTTP para uma API externa de SSO para validar se o domínio está autorizado e obter o TenantId. Quero remover essa dependência externa e trazer toda essa lógica para dentro da API principal.

---

## Objetivo
Refatorar o sistema para:

1. Validar domínio internamente (sem chamada HTTP)
2. Centralizar o conceito de Tenant
3. Implementar sistema de planos por estabelecimento
4. Controlar limite de barbeiros por plano

---

## Diretrizes importantes

- A modelagem das entidades (Tenant, Domain, Plan, etc.) deve ser definida livremente, da forma mais adequada e escalável
- Pode criar relacionamentos, propriedades e estruturas conforme boas práticas de DDD e EF Core
- Evitar hardcode de regras (planos devem ser configuráveis)

---

## Requisitos funcionais

### 1. Validação de Domínio

- O sistema deve validar o domínio recebido na requisição (`HttpContext.Request.Origin`)
- Deve existir uma estrutura persistida que relacione domínio com tenant
- O domínio deve ter um indicador de autorização
- Caso não autorizado → retornar 403

---

### 2. Resolução de Tenant

- A partir do domínio, deve ser possível obter o TenantId
- O TenantId deve ser disponibilizado durante o ciclo da requisição (ex: TenantProvider)

---

### 3. Remoção do SSO

- Remover completamente a chamada HTTP para a API externa de SSO
- Toda validação deve ser feita internamente via banco de dados

---

### 4. Sistema de Planos

- Cada tenant deve possuir um plano
- O plano define limites de uso do sistema
- Neste momento, o limite relevante é:
  - Quantidade máxima de barbeiros cadastrados

#### Planos esperados:

- Plano 1 → permite até 1 barbeiro
- Plano 2 → permite até 3 barbeiros
- Plano 3 → permite até 5 barbeiros

**Observação:**
- A estrutura deve permitir fácil evolução (novos limites, novas features, etc.)

---

### 5. Validação ao cadastrar barbeiro

Antes de cadastrar um barbeiro:

- Identificar o tenant atual
- Consultar o plano do tenant
- Verificar quantos barbeiros já existem para esse tenant
- Bloquear operação caso atinja o limite

Mensagem sugerida:

> "Seu plano atual não permite cadastrar mais barbeiros."

---

### 6. Middleware

Refatorar o `TenantValidationMiddleware` para:

- Ler o domínio da request
- Validar no banco
- Resolver o TenantId
- Injetar o TenantId em um provider acessível na aplicação
- Continuar pipeline normalmente

---

### 7. Infraestrutura

- Criar entidades necessárias
- Criar migrations
- Criar services/repositories conforme necessário
- Garantir uso de async/await
- Aplicar boas práticas de organização de código

---

### 8. Boas práticas

- Garantir isolamento por tenant
- Evitar queries sem filtro de tenant
- Considerar uso de filtros globais no EF Core
- Estrutura preparada para escalar como SaaS

---

## Objetivo final

Eliminar dependência externa de SSO, centralizar multi-tenant na aplicação e implementar controle de plano de forma robusta e escalável.
