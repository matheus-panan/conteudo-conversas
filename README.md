# 📊 Painel de conversas SURI

Um sistema simples feito em **ASP.NET Core MVC** para **visualizar o conteúdo das conversas dos clientes** a partir de uma API.

## ✨ Funcionalidades

- Conecta a uma API protegida por token (Bearer).
- Recupera e exibe mensagens de conversas no formato de tabela.
- Mostra detalhes como:
  - ID da mensagem
  - Texto da conversa
  - Origem 
  - Remetente
  - Data formatada
  - Tipo da mensagem (ex: SystemMessage, AgentMessage)

## 🧰 Tecnologias utilizadas

- ASP.NET Core 8 (MVC)
- C#
- Razor Pages
- HttpClient para consumo da API
- System.Text.Json para desserialização

## 📁 Organização do projeto

- `Models/Chat.cs` – Representa uma mensagem individual.
- `Models/ChatResponse.cs` – Representa a estrutura da resposta da API.
- `Services/ApiService.cs` – Faz a requisição HTTP e retorna os dados tratados.
- `Controllers/ChatController.cs` – Controla a lógica da tela.
- `Views/Chat/Index.cshtml` – Exibe a lista de mensagens formatadas.

## 🛠 Como executar

### 1. Clone o repositório:

```bash
git clone https://github.com/seu-usuario/painel_conversas.git
```
### 2. Acesse a pasta do projeto
```bashe
cd painel_conversas
```
### 3. Abra o projeto no Visual Studio, Visual Studio Code ou Rider

### 4. Atualize a URL da API e o token em ApiService.cs, se necessário:
### Arquivo: Services/ApiService.cs

request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "SEU_TOKEN_AQUI");

### 5. Execute o projeto
### No Visual Studio: pressione Ctrl + F5
### Ou via terminal:
```bash
dotnet run
```
