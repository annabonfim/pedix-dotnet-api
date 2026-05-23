# 🍝 Pedix API — Atendimentos (.NET)

API REST em **ASP.NET Core 8** que cuida do **atendimento em restaurante**: autenticação por papel (cliente, garçom, gerente), mesas, pedidos, itens-pedido e pagamentos. Persistência em **Oracle**, arquitetura **Clean Architecture** + **DDD**, JWT com BCrypt.

É a API consumida pelo app mobile **Pedix** ([github.com/annabonfim/pedix-app](https://github.com/annabonfim/pedix-app)) — junto com a [API Java](https://github.com/alanerochaa/pedix-api) que serve o cardápio.

> Sprint 4 / Challenge FIAP 2026 — CodeGirls 👩‍💻

---

## 🧭 Índice

1. [Visão geral](#-visão-geral)
2. [Arquitetura](#%EF%B8%8F-arquitetura)
3. [Tecnologias](#%EF%B8%8F-tecnologias)
4. [Estrutura do projeto](#%EF%B8%8F-estrutura-do-projeto)
5. [Entidades](#%EF%B8%8F-entidades)
6. [Como rodar](#-como-rodar)
7. [Endpoints](#-endpoints)
8. [Autenticação](#-autenticação)
9. [Regras de negócio](#-regras-de-negócio)
10. [Observabilidade](#-observabilidade-health-logs-tracing)
11. [Testes](#-testes-automatizados)
12. [Time](#-time)

---

## 🎯 Visão geral

A API gerencia tudo que acontece **na mesa** do restaurante:

- Cliente faz login no app → seleciona uma mesa (QR code) → faz pedidos
- Pedido cria comanda automática e marca a mesa como `OCUPADA`
- Garçom acompanha pedidos da mesa e avança status (`ABERTO → EM_PREPARO → PRONTO → ENTREGUE`)
- Cliente paga a conta (PIX, crédito, débito ou dinheiro) → API aprova, marca pedidos como ENTREGUE em cascata e libera a mesa

Cardápio, categorias, avaliações e histórico vivem em outra API (Java), por divisão de responsabilidade do squad.

---

## 🏗️ Arquitetura

Clean Architecture com 4 camadas. Sem acoplamento entre Domain e Infraestrutura — o repositório é uma interface no Domain implementada na Infrastructure.

```
┌───────────────────────────────────────────────┐
│  Atendimentos.Api          (Controllers/HTTP) │
└────────────────────┬──────────────────────────┘
                     │
┌────────────────────▼──────────────────────────┐
│  Atendimentos.Application  (Services / DTOs)  │
└────────────────────┬──────────────────────────┘
                     │
┌────────────────────▼──────────────────────────┐
│  Atendimentos.Domain       (Entities + I*)    │
└────────────────────┬──────────────────────────┘
                     │
┌────────────────────▼──────────────────────────┐
│  Atendimentos.Infrastructure (EF Core/Oracle) │
└───────────────────────────────────────────────┘
```

---

## ⚙️ Tecnologias

| Categoria | Stack |
|---|---|
| Linguagem | C# (.NET 8) |
| Web | ASP.NET Core Web API |
| Auth | JWT (HS256) + BCrypt.Net-Next |
| ORM | Entity Framework Core (Oracle.EntityFrameworkCore) |
| Banco | Oracle (FIAP Cloud) |
| Doc | Swagger / Swashbuckle |
| HATEOAS | Helper próprio |
| Logs | Serilog (console + arquivo) |
| Tracing | OpenTelemetry |
| Health | AspNetCore.HealthChecks |
| Testes | xUnit + Moq + WebApplicationFactory |

---

## 🗂️ Estrutura do projeto

```
src/
├── Atendimentos.Api/                # camada HTTP
│   ├── Controllers/
│   │   ├── Auth/AuthController.cs   # login + register por papel
│   │   ├── ClientesController.cs
│   │   ├── GarconsController.cs
│   │   ├── MesasController.cs
│   │   ├── PedidosController.cs     # CRUD + atualizar status
│   │   ├── PedidoItensController.cs # itens de cada pedido
│   │   ├── PagamentosController.cs  # criar / aprovar / recusar
│   │   └── ComandasController.cs    # legado, mantido p/ compat
│   ├── Helpers/HateoasHelper.cs
│   └── Program.cs                   # DI, JWT, Serilog, OpenTelemetry
│
├── Atendimentos.Application/
│   ├── DTOs/
│   │   ├── Auth/                    # RegisterClienteDto, LoginDto, ...
│   │   ├── AtualizarStatusPedidoDto.cs
│   │   └── CriarPagamentoDto.cs
│   └── Services/
│       ├── Auth/AuthService.cs      # BCrypt + JWT
│       ├── PedidoService.cs         # ocupa/libera mesa em cascata
│       ├── PedidoItemService.cs
│       └── PagamentoService.cs      # aprovar → fechar conta
│
├── Atendimentos.Domain/
│   ├── Entities/                    # Cliente, Garcom, Mesa, Pedido, ...
│   ├── Enums/                       # MesaStatus, UsuarioRole, ...
│   └── Repositories/                # I*Repository (interfaces)
│
└── Atendimentos.Infrastructure/
    ├── Context/AtendimentosDbContext.cs
    ├── Migrations/
    └── Repositories/                # implementações EF Core
```

---

## 🗃️ Entidades

| Entidade | Campos principais |
|---|---|
| **Usuario** | `Id`, `Nome`, `Email`, `SenhaHash` (BCrypt), `Role` (Cliente/Garcom/Admin), `DataNascimento`, `Telefone`, `CPF`, `Matricula`, `AdminKey` |
| **Cliente / Garcom** | Specializações de Usuario com campos próprios |
| **Mesa** | `Numero`, `Capacidade`, `Status` (Livre/Ocupada/AguardandoAtendimento), `Localizacao`, `QrCode` |
| **Pedido** | `ClienteId`, `GarcomId`, `MesaId`, `DataPedido`, `ValorTotal`, `Status` (ABERTO → EM_PREPARO → PRONTO → ENTREGUE; ou CANCELADO) |
| **PedidoItem** | `PedidoId`, `ItemCardapioId` (FK lógico p/ API Java), `Quantidade`, `PrecoMomento`, `Subtotal` |
| **Pagamento** | `PedidoId`, `Valor`, `MetodoPagamento` (PIX/CREDITO/DEBITO/DINHEIRO), `Status` (PENDENTE → APROVADO/RECUSADO) |

---

## 🚀 Como rodar

### Pré-requisitos
- .NET 8 SDK
- Acesso a um banco Oracle (FIAP Cloud serve)

### 1) Clone e restore
```bash
git clone https://github.com/annabonfim/pedix-dotnet-api.git
cd pedix-dotnet-api
dotnet restore
```

### 2) Configure o banco
Crie `src/Atendimentos.Api/appsettings.Development.json` (gitignored):
```json
{
  "ConnectionStrings": {
    "OracleDb": "Data Source=oracle.fiap.com.br:1521/ORCL;User Id=rmXXXXXX;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Key": "uma_chave_secreta_de_pelo_menos_32_caracteres_aqui",
    "Issuer": "Atendimentos.Api",
    "Audience": "Atendimentos.Client"
  },
  "AdminSettings": {
    "AdminKey": "SUA_CHAVE_MASTER_PARA_REGISTRAR_ADMIN"
  }
}
```

### 3) Aplique as migrations
```bash
dotnet ef database update \
  --project src/Atendimentos.Infrastructure \
  --startup-project src/Atendimentos.Api
```

### 4) Suba a API
```bash
dotnet run --project src/Atendimentos.Api --urls "http://0.0.0.0:5070"
```

Acesse:
- Swagger: <http://localhost:5070/swagger>
- Health: <http://localhost:5070/health>

### Build pra mobile (mesma LAN)
Se o app mobile rodar em celular físico, use o IP do Mac (não `localhost`):
```bash
dotnet run --project src/Atendimentos.Api --urls "http://0.0.0.0:5070"
# o app aponta para http://<seu-ip-local>:5070/api
```

---

## 🌐 Endpoints

### 🔐 Auth (`/api/auth`)
| Método | Rota | Descrição |
|---|---|---|
| POST | `/register-cliente` | Cadastra cliente |
| POST | `/register-garcom` | Cadastra garçom |
| POST | `/register-admin` | Cadastra admin (exige `adminKey`) |
| POST | `/login-cliente` | Login + JWT (valida que role é Cliente) |
| POST | `/login-garcom` | Login + JWT (valida role Garcom) |
| POST | `/login-admin` | Login + JWT (valida role Admin) |

### 🪑 Mesas (`/api/mesas`)
| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Lista mesas (com status) |
| GET | `/{id}` | Detalhe da mesa |
| POST | `/` | Cria mesa |
| PUT | `/{id}` | Atualiza dados da mesa |
| PUT | `/{id}/status` | Atualiza só o status (`0`=Livre, `1`=Ocupada, `2`=AguardandoAtendimento) |
| DELETE | `/{id}` | Remove mesa |

### 🧾 Pedidos (`/api/pedidos`)
| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Lista todos |
| GET | `/{id}` | Detalhe |
| GET | `/cliente/{clienteId}` | Pedidos do cliente (comanda dele) |
| GET | `/mesa/{mesaId}` | Pedidos da mesa (visão garçom) |
| GET | `/garcom/{garcomId}` | Pedidos atendidos por um garçom |
| POST | `/?clienteId=X&garcomId=Y&mesaId=Z` | Cria pedido vazio — automaticamente marca mesa como **OCUPADA** |
| PUT | `/{id}/status` | Avança status. Quando vai pra `ENTREGUE`, se a mesa não tem mais pedido ativo, ela volta pra **LIVRE** |

### 🍽️ Itens-Pedido (`/api/pedido-itens`)
| Método | Rota | Descrição |
|---|---|---|
| GET | `/pedido/{pedidoId}` | Itens de um pedido |
| POST | `/?pedidoId=X&itemCardapioId=Y&quantidade=N&precoMomento=P` | Adiciona item (preço congelado no momento do pedido) |
| DELETE | `/{id}` | Remove item |

### 💳 Pagamentos (`/api/pagamentos`)
| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Lista todos |
| GET | `/{id}` | Detalhe |
| GET | `/pedido/{pedidoId}` | Pagamentos de um pedido |
| POST | `/?pedidoId=X&valor=V&metodoPagamento=PIX` | Cria pagamento PENDENTE |
| PUT | `/{id}/aprovar` | Aprova → **fecha a conta** marcando os pedidos do cliente naquela mesa como ENTREGUE em cascata |
| PUT | `/{id}/recusar` | Marca como RECUSADO |

---

## 🔐 Autenticação

JWT assinado com HS256, BCrypt no hash da senha.

### Fluxo
1. Cliente: `POST /api/auth/register-cliente` → cria
2. Cliente: `POST /api/auth/login-cliente` → recebe `{ token }`
3. Mobile guarda o token e manda no header de cada request:
   ```
   Authorization: Bearer eyJhbGciOi...
   ```

### Claims dentro do token
| Claim | Valor |
|---|---|
| `nameidentifier` | UUID do usuário |
| `name` | Nome |
| `emailaddress` | E-mail |
| `role` | `Cliente` / `Garcom` / `Admin` |
| `exp` | Expira em 7 dias |

### Por que login separado por papel?
A tela de login no app força a escolha do perfil. Os endpoints `/login-cliente`, `/login-garcom`, `/login-admin` validam que o usuário **realmente é desse papel** — assim um cliente não consegue se passar por garçom só com email/senha dele.

### AdminKey
`POST /api/auth/register-admin` exige um campo `adminKey` que precisa bater com o valor em `appsettings.json:AdminSettings.AdminKey`. Sem isso, qualquer um podia se registrar como admin.

---

## 🧠 Regras de negócio

- **Mesa OCUPADA automática**: ao criar pedido, `MesaService.AlterarStatus(Ocupada)` é chamado em cascata.
- **Mesa LIVRE automática**: quando um pedido vira `ENTREGUE`, o `PedidoService` checa se ainda existe algum pedido ativo (não-ENTREGUE e não-CANCELADO) naquela mesa. Se não tem mais nada ativo → mesa volta pra `LIVRE`.
- **CANCELADO não libera mesa**: cliente pode ter cancelado um item mas continuar à mesa pedindo outras coisas.
- **Pagamento APROVADO fecha conta**: ao aprovar pagamento, todos os pedidos ativos do mesmo cliente naquela mesa são marcados como `ENTREGUE` em cascata, o que dispara a liberação da mesa.
- **Comanda por cliente, não por mesa**: cada cliente tem sua própria sequência de pedidos. Privacidade entre clientes da mesma mesa e pagamento individual sem precisar dividir conta.
- **Status válidos do pedido**: `ABERTO`, `EM_PREPARO`, `PRONTO`, `ENTREGUE`, `CANCELADO`. Qualquer outro valor é rejeitado por validação.

---

## 📈 Observabilidade (Health, Logs, Tracing)

### Health Check
```
GET /health
```
Retorna status da API + conexão com banco.

### Logs estruturados (Serilog)
- Console em dev
- Arquivo rotativo em `logs/log-YYYY-MM-DD.txt`
- Inclui método HTTP, rota, status, latência

### Tracing distribuído (OpenTelemetry)
- Cada request gera um `TraceId`
- Exportado pra console em dev

---

## 🧪 Testes automatizados

Padrão **AAA** (Arrange / Act / Assert), com xUnit + Moq.

```bash
dotnet test
```

Cobre:
- **Unit**: services (criar cliente, alterar status de pedido, aprovar pagamento)
- **Integration**: `WebApplicationFactory` + EF Core InMemory, testando os endpoints de ponta a ponta

---

## 👥 Time

| Nome | RM | Função |
|---|---|---|
| Maria Eduarda Araujo Penas | RM560944 | Backend / Infra |
| Alane Rocha da Silva | RM561052 | Backend (API Java de cardápio) |
| Anna Beatriz de Araujo Bonfim | RM559561 | Mobile + integração + auth/pedido/pagamento |

🎓 **FIAP — 2TDSPS — Challenge Oracle 2026 — CodeGirls**
