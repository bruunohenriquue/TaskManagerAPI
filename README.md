# Task Manager API

API REST em **.NET 8** para gestão simples de tarefas: cadastro, listagem com filtros, edição, exclusão e busca por id. Os dados ficam em memória (**Entity Framework Core InMemory**), adequado para desenvolvimento e demonstração sem banco real.

## Arquitetura

A solução separa responsabilidades em camadas:

- **TaskManager.Domain:** entidades e enum de status (`TaskItem`, `TaskItemStatus`), sem dependência de infraestrutura.
- **TaskManager.Application:** DTOs, contratos (`ITaskRepository`), `TaskService` e exceções de aplicação — a regra de uso fica aqui, sem EF.
- **TaskManager.Infrastructure:** `AppDbContext`, `TaskRepository` e registro do EF InMemory.
- **TaskManager.Api:** controllers, Swagger, validação de entrada, logging e tratamento global de erros (`ProblemDetails`).

Injeção de dependência concentra-se em `DependencyInjection` por projeto. Isso segue **SOLID** (especialmente inversão de dependência): a aplicação define o repositório; a infraestrutura implementa.

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Como executar

Na raiz do repositório:

```powershell
dotnet restore TaskManager.slnx
dotnet build TaskManager.slnx
dotnet run --project TaskManager.Api\TaskManager.Api.csproj
```

Com o perfil padrão, a API sobe em **http://localhost:5258** (veja [TaskManager.Api/Properties/launchSettings.json](TaskManager.Api/Properties/launchSettings.json)).

- **Swagger UI:** http://localhost:5258/swagger  
- Especificação OpenAPI: http://localhost:5258/swagger/v1/swagger.json  

O JSON serializa o enum de status como string (`Pendente`, `EmProgresso`, `Concluida`).

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/api/tasks` | Cria tarefa. Corpo: `title` (obrigatório), `description`, `dueDate`, `status`. Resposta **201** com `id` (GUID). |
| `GET` | `/api/tasks` | Lista tarefas. Query opcional: `status`, `dueDate` (filtra pelo **dia civil** do vencimento). |
| `GET` | `/api/tasks/{id}` | Obtém uma tarefa. **404** se não existir. |
| `PUT` | `/api/tasks/{id}` | Atualiza tarefa. **404** se não existir. |
| `DELETE` | `/api/tasks/{id}` | Remove tarefa. **204**; **404** se não existir. |

Validação inválida retorna **400** (detalhes no corpo padrão do ASP.NET Core). Erros não tratados retornam **500** com `ProblemDetails`.

## Logging

Com a API rodando no terminal (ou outro sink configurado), aparecem registos em situações de erro:

- **400** (modelo inválido): categoria `Api.Validation`, nível **Warning**, com método HTTP e caminho.
- **404** (tarefa inexistente): `GlobalExceptionHandler`, **Warning**, com método, caminho e id.
- **500** (exceção não tratada): `GlobalExceptionHandler`, **Error**, com método, caminho e stack da exceção.

Operações bem-sucedidas de criação/alteração/exclusão também escrevem linhas **Information** no `TasksController`.

## Exemplos (curl)

Substitua `{id}` pelo GUID retornado na criação.

```bash
curl -s -X POST http://localhost:5258/api/tasks -H "Content-Type: application/json" -d "{\"title\":\"Revisar README\",\"status\":\"Pendente\"}"
curl -s "http://localhost:5258/api/tasks?status=Pendente"
curl -s "http://localhost:5258/api/tasks/{id}"
curl -s -X PUT http://localhost:5258/api/tasks/{id} -H "Content-Type: application/json" -d "{\"title\":\"Revisar README\",\"status\":\"EmProgresso\"}"
curl -s -X DELETE http://localhost:5258/api/tasks/{id}
```

## Testes automatizados

```powershell
dotnet test TaskManager.slnx
```

O projeto **TaskManager.Tests** usa **xUnit** e **Moq** para testar `TaskService` com repositório mockado.
