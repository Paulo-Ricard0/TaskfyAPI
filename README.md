<h3 align="center">
    <a href="https://taskfy.apidocumentation.com/"><img alt="Banner Taskfy" title="taskfy" src="https://firebasestorage.googleapis.com/v0/b/uploads-58ebc.appspot.com/o/TaskfyLogo.png?alt=media&token=c3ef71c9-4a7d-493e-bc2c-1589a45d02be" /></a>
</h3>

<h1 align="center">📝 TASKFY</h1>

<h3 align="center">Um sistema em .NET para gerenciamento de tarefas</h3>

<div align="center">
  <img alt="Repository size" src="https://img.shields.io/github/repo-size/Paulo-Ricard0/TaskfyAPI?color=618f61">
  
  <a href="https://github.com/Paulo-Ricard0/TaskfyAPI/commits/main">
    <img alt="GitHub last commit" src="https://img.shields.io/github/last-commit/Paulo-Ricard0/TaskfyAPI?color=618f61">
  </a>
</div>

<h4 align="center">
   Status: Concluído 🚀
</h4>

---

## 📋 Índice
- [Visão Geral](#-visão-geral)
- [Documentação](#-documentação)
- [Diagrama de Sequência do sistema](#-diagrama-de-sequência-do-sistema)
- [Funcionalidades](#%EF%B8%8F-funcionalidades)
- [Testes](#-testes)
- [Tecnologias utilizadas](#%EF%B8%8F-tecnologias-utilizadas)
- [Autor](#-autor)

---

## 💻 Visão Geral

Uma Web API desenvolvida em .NET 8 para o gerenciamento de tarefas e autenticação de usuários. Ela permite a criação, atualização, consulta e exclusão de tarefas, bem como o registro e login de usuários, e inclui um sistema de logs e tratamento de erros.

---

## 📚 Documentação
- Para visualizar a documentação de todas as rotas da API, [clique aqui](https://taskfy.apidocumentation.com/)

---

## 🧩 Diagrama de Sequência do sistema

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant Backend
    participant Database

    User->>Frontend: Preenche formulário de registro
    Frontend->>Backend: POST /api/users/register (dados do usuário)
    Backend->>Backend: Valida dados de registro
    Backend->>Database: Verifica se e-mail já está em uso
    Backend->>Database: Insere novo usuário
    Backend-->>Frontend: 201 Created (mensagem confirmação) / 400 Bad Request / 409 Conflict
    Frontend-->>User: Redireciona para página de login

    User->>Frontend: Preenche formulário de login
    Frontend->>Backend: POST /api/users/login (e-mail, senha)
    Backend->>Backend: Valida dados de login
    Backend->>Database: Verifica credenciais
    Backend-->>Frontend: 200 OK (token JWT) / 401 Unauthorized / 400 Bad Request
    Frontend-->>User: Redireciona para página principal

    User->>Frontend: Acessa página principal
    Frontend->>Backend: GET /api/tasks/ (token JWT)
    Backend->>Backend: Valida token JWT
    Backend->>Database: Recupera lista de tarefas
    Database-->>Backend: Lista de tarefas
    Backend-->>Frontend: 200 OK (lista de tarefas) / 401 Unauthorized / 404 Not Found
    Frontend-->>User: Exibe lista de tarefas

    User->>Frontend: Clica em adicionar nova tarefa
    Frontend->>User: Exibe formulário de nova tarefa
    User->>Frontend: Preenche formulário de nova tarefa
    Frontend->>Backend: POST /api/tasks/ (detalhes da tarefa, token JWT)
    Backend->>Backend: Valida token JWT
    Backend->>Backend: Valida dados da tarefa
    Backend->>Database: Insere nova tarefa
    Backend-->>Frontend: 201 Created (objeto JSON da tarefa) / 400 Bad Request / 401 Unauthorized
    Frontend-->>User: Exibe a nova tarefa na lista

    User->>Frontend: Clica em editar tarefa existente
    Frontend->>Backend: GET /api/tasks/{taskId} (token JWT)
    Backend->>Backend: Valida token JWT
    Backend->>Database: Recupera detalhes da tarefa
    Database-->>Backend: Detalhes da tarefa
    Backend-->>Frontend: 200 OK (detalhes da tarefa) / 401 Unauthorized / 404 Not Found
    Frontend-->>User: Exibe formulário preenchido
    User->>Frontend: Edita e envia o formulário
    Frontend->>Backend: PUT /api/tasks/{taskId} (novos detalhes da tarefa, token JWT)
    Backend->>Backend: Valida token JWT
    Backend->>Backend: Valida dados da tarefa
    Backend->>Database: Atualiza tarefa
    Backend-->>Frontend: 200 OK (objeto JSON atualizado) / 401 Unauthorized / 403 Forbidden / 404 Not Found / 400 Bad Request
    Frontend-->>User: Exibe a tarefa atualizada na lista

    User->>Frontend: Clica em excluir tarefa existente
    Frontend->>Backend: DELETE /api/tasks/{taskId} (token JWT)
    Backend->>Backend: Valida token JWT
    Backend->>Database: Exclui tarefa
    Backend-->>Frontend: 200 OK / 401 Unauthorized / 403 Forbidden / 404 Not Found
    Frontend-->>User: Remove a tarefa da lista
```

---

## ⚙️ Funcionalidades

- **Registro de Usuário**: Permite que novos usuários se registrem na aplicação utilizando nome de usuário, e-mail e senha.
- **Login de Usuário**: Autentica com e-mail e senha usuários já registrados.
- **CRUD de Tarefas**: Permite que usuários criem, busquem, atualizem e excluam suas tarefas.
- **Envio de e-mails**: Envia e-mails de notificação para o usuário.
- **Middleware Global de Erros**: Captura e trata erros, fornecendo respostas adequadas ao cliente e registrando logs.
- **Sistema de Logs**: Monitora e registra erros e eventos importantes na aplicação.

---

## ✉️ Fluxo de mensageria

```mermaid
flowchart TD
    A[Usuario] --> |Realiza uma ação| B[Taskfy.API]
    B --> C[Publica uma notificação no RabbitMQ]
    C --> D[Exchange]
    D --> E[taskfy_notification_queue]
    E --> |Consome a fila| F[Taskfy.NotificationService]
    F --> |Cria o template de email e publica na fila| G[Exchange]
    G --> H[taskfy_email_queue]
    H --> |Consome a fila| I[Taskfy.EmailService]
    I --> |Envia e-mail| J[Destinatário]
    
    subgraph RabbitMQ_______________
        D
        E
        G
        H
    end
    
    subgraph Serviços
        F
        I
    end                                                
```

## 🧪 Testes
A aplicação inclui uma suíte de testes unitários utilizando o xUnit, cobrindo todos os casos relevantes nos controllers e services de usuários e tarefas.
- **Total de Testes**: 41
- **Cobertura**: Controllers e Services de Usuários e Tarefas

<img alt="Imagem com testes unitarios" title="testes unitarios" src="https://firebasestorage.googleapis.com/v0/b/uploads-58ebc.appspot.com/o/Captura%20de%20tela%202024-06-17%20004144.png?alt=media&token=5a97515b-1e0d-4489-925b-07f230ba95bc" />

---

## 🛠️ Tecnologias utilizadas

### Aplicação:
- .NET 8
- Entity Framework Core
- AspNetCore Identity
- JWT
- AutoMapper
- Swagger

### Mensageria e envio de e-mails:
- RabbitMQ
- MailKit

### Banco de dados:
- SQL Server

### Testes unitários:
- xUnit
- NSubstitute
- FluentAssertions

---

## 🧑‍💻 Autor

Esse projeto foi desenvolvido por:

<table>
  <tr>
    <td align="center"><a href="https://www.linkedin.com/in/paulo-ricardo-magalh%C3%A3es/"><img src="https://firebasestorage.googleapis.com/v0/b/quiz-baleias.appspot.com/o/ultima2.jpg?alt=media&token=68c74a20-9738-4d63-9aaf-b02608678c93" width="80px" alt="Foto Paulo Ricardo"/><br /><sub><b>Paulo Ricardo</b></sub></a><br /></td>
  </tr>
</table>

<p align="right"><a href="#top"><img src="https://img.shields.io/static/v1?label&message=voltar+ao+topo&color=618f61&style=flat&logo" alt="voltar ao topo" /></a></p>
