# JobHunter AI

O **JobHunter AI** é uma aplicação desenvolvida para automatizar parte do processo de busca e análise de vagas de tecnologia.

O sistema busca oportunidades em fontes externas, elimina vagas já analisadas, utiliza Inteligência Artificial para comparar os requisitos da vaga com o perfil profissional do candidato e classifica cada oportunidade de acordo com o nível de compatibilidade.

Além disso, as análises são armazenadas em PostgreSQL e podem ser visualizadas em um dashboard desenvolvido em React.

---

## Objetivo

O projeto surgiu com a ideia de criar um assistente inteligente para auxiliar candidatos durante a busca por oportunidades de emprego.

Em vez de analisar manualmente diversas vagas, o sistema realiza o seguinte fluxo:

```text
Busca de vagas
      ↓
Filtragem
      ↓
Remoção de duplicatas
      ↓
Comparação com o perfil do candidato
      ↓
Análise utilizando IA
      ↓
Cálculo de compatibilidade
      ↓
Classificação da vaga
      ↓
Persistência no PostgreSQL
      ↓
Exibição no dashboard
```

---

## Funcionalidades

Atualmente o sistema possui:

- Busca de vagas utilizando APIs externas
- Integração com Jooble
- Integração com Remotive
- Filtro de vagas de tecnologia
- Filtro de senioridade
- Remoção de vagas duplicadas
- Detecção de vagas já analisadas
- Análise de compatibilidade utilizando IA
- Identificação do nível da vaga
- Justificativa da análise
- Classificação automática da oportunidade
- Persistência das análises em PostgreSQL
- Dashboard em React
- Busca de vagas por cargo ou empresa
- Filtros por recomendação
- Ordenação por compatibilidade
- Cards expansíveis com detalhes da análise
- Link direto para a vaga original
- Controle de vagas visualizadas
- Busca e análise de novas vagas diretamente pelo dashboard
- Seleção da quantidade de vagas que serão analisadas
- Cache das buscas do Jooble para reduzir chamadas à API

---

## Classificação das vagas

Após a análise da IA, a aplicação utiliza a compatibilidade calculada para definir a recomendação:

| Compatibilidade | Recomendação |
|---|---|
| 80% a 100% | `CANDIDATAR` |
| 60% a 79% | `ANALISAR` |
| Abaixo de 60% | `IGNORAR` |

A decisão final de candidatura continua sendo do usuário. A classificação serve como apoio para priorizar oportunidades.

---

## Dashboard

O dashboard apresenta um resumo das vagas analisadas:

- Total de vagas
- Quantidade recomendada para candidatura
- Quantidade que precisa de análise
- Quantidade recomendada para ignorar
- Média de compatibilidade

Também é possível:

- Filtrar pelas recomendações
- Pesquisar por cargo ou empresa
- Ordenar automaticamente pela maior compatibilidade
- Expandir uma vaga para visualizar os detalhes
- Consultar a justificativa gerada pela IA
- Acessar a vaga original
- Marcar uma vaga como visualizada
- Buscar novas vagas sem utilizar o terminal

---

## Tecnologias utilizadas

### Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Npgsql
- PostgreSQL
- OpenAI API
- Jooble API
- Remotive API
- Memory Cache

### Frontend

- React
- TypeScript
- Vite
- CSS

### Infraestrutura

- Docker
- Docker Compose
- PostgreSQL
- Git
- GitHub

---

## Arquitetura

A aplicação é dividida em frontend, backend e banco de dados.

```text
React + TypeScript
        ↓
ASP.NET Core Web API
        ↓
Services
 ├── OpenAIService
 ├── JoobleService
 ├── RemotiveService
 └── VagaSearchService
        ↓
Entity Framework Core
        ↓
PostgreSQL
```

As APIs externas são utilizadas pelo backend para buscar vagas e realizar as análises.

---

## Estrutura do projeto

```text
JobHunterAI/
│
├── backend/
│   └── JobHunterAI.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── Migrations/
│       ├── Models/
│       ├── Services/
│       └── Program.cs
│
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── App.tsx
│   │   ├── App.css
│   │   └── index.css
│   └── package.json
│
├── compose.yaml
└── README.md
```

---

## Banco de dados

O PostgreSQL é executado através do Docker.

O projeto utiliza:

```text
Database: jobhunterai
Porta local: 5433
```

Para iniciar o banco:

```bash
docker compose up -d
```

Para verificar os containers:

```bash
docker ps
```

---

## Configuração do backend

Entre na pasta raiz do projeto e configure a conexão com o PostgreSQL utilizando User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=jobhunterai;Username=postgres;Password=postgres" --project backend/JobHunterAI.Api
```

Configure também a chave da OpenAI:

```bash
dotnet user-secrets set "OpenAI:ApiKey" "SUA_CHAVE" --project backend/JobHunterAI.Api
```

E a chave da Jooble:

```bash
dotnet user-secrets set "Jooble:ApiKey" "SUA_CHAVE" --project backend/JobHunterAI.Api
```

> As chaves das APIs não devem ser armazenadas diretamente no código ou enviadas para o GitHub.

---

## Migrations

Para aplicar as migrations existentes:

```bash
dotnet ef database update --project backend/JobHunterAI.Api --startup-project backend/JobHunterAI.Api
```

Para visualizar as migrations:

```bash
dotnet ef migrations list --project backend/JobHunterAI.Api --startup-project backend/JobHunterAI.Api
```

---

## Executando o backend

Na raiz do projeto:

```bash
dotnet run --project backend/JobHunterAI.Api
```

Por padrão, durante o desenvolvimento, a API é executada em:

```text
http://localhost:5245
```

---

## Executando o frontend

Abra outro terminal:

```bash
cd frontend
```

Instale as dependências:

```bash
npm install
```

Execute:

```bash
npm run dev
```

O frontend ficará disponível em:

```text
http://localhost:5173
```

---

## Principais endpoints

### Listar vagas

```http
GET /api/vagas
```

### Buscar vaga por ID

```http
GET /api/vagas/{id}
```

### Buscar vagas externas

```http
GET /api/vagas/buscar-todas
```

### Buscar vagas no Jooble

```http
GET /api/vagas/buscar-jooble
```

### Buscar vagas no Remotive

```http
GET /api/vagas/buscar
```

### Buscar, analisar e salvar novas vagas

```http
POST /api/vagas/analisar-e-salvar?limite=3
```

### Vagas recomendadas para candidatura

```http
GET /api/vagas/candidatar
```

### Vagas para analisar

```http
GET /api/vagas/analisar
```

### Vagas para ignorar

```http
GET /api/vagas/ignorar
```

### Resumo do dashboard

```http
GET /api/vagas/resumo
```

### Marcar vaga como visualizada

```http
PATCH /api/vagas/{id}/visualizada
```

---

## Exemplo de análise

Uma análise pode retornar dados semelhantes a:

```json
{
  "compatibilidade": 82,
  "recomendacao": "CANDIDATAR",
  "pontosFortes": [
    "C#",
    ".NET",
    "APIs REST",
    "SQL"
  ],
  "conhecimentosAusentes": [
    "Experiência profissional específica exigida pela vaga"
  ],
  "nivelVaga": "Júnior",
  "justificativa": "O candidato apresenta boa compatibilidade técnica e de senioridade com a oportunidade."
}
```

---

## Cache da busca de vagas

Para evitar chamadas desnecessárias à API da Jooble, o backend utiliza `IMemoryCache`.

O resultado de uma busca permanece armazenado por aproximadamente:

```text
30 minutos
```

Durante esse período, novas análises reutilizam as vagas encontradas anteriormente e verificam quais ainda não foram analisadas.

Isso reduz o consumo da API externa e melhora o tempo de resposta.

---

## Controle de duplicidade

Antes de enviar uma vaga para análise pela IA, o sistema verifica se ela já está registrada no banco.

```text
Vaga encontrada
      ↓
Já existe no banco?
   ├── Sim → ignora
   └── Não → analisa com IA
                  ↓
                 salva
```

Isso evita análises repetidas e reduz o consumo da API de Inteligência Artificial.

---

## Melhorias futuras

Algumas funcionalidades planejadas para evolução do projeto:

- Cadastro e edição do perfil profissional
- Upload de currículo
- Extração automática de informações do currículo
- Persistência dos pontos fortes e conhecimentos ausentes
- Status de candidatura
- Histórico de candidaturas
- Novas fontes de vagas
- Configuração de localização e modalidade
- Configuração das tecnologias desejadas
- Paginação
- Autenticação
- Testes automatizados
- Tratamento global de erros
- Deploy do frontend e backend
- Execução automática de buscas em intervalos configuráveis

---

## Motivação

A busca por vagas normalmente exige acessar diferentes plataformas, ler diversas descrições e identificar manualmente quais oportunidades possuem maior relação com o perfil do candidato.

O JobHunter AI foi desenvolvido como um projeto de portfólio para explorar a integração entre:

- desenvolvimento backend
- desenvolvimento frontend
- APIs externas
- Inteligência Artificial
- banco de dados
- Docker
- automação

O objetivo é demonstrar como essas tecnologias podem trabalhar juntas para resolver um problema real.

---

## Autor

**Matheus Leite**

Estudante de Engenharia de Software.

GitHub: [mathelico](https://github.com/mathelico)

---

## Status do projeto

🚧 **Em desenvolvimento**

O projeto já possui o fluxo principal funcionando:

```text
Buscar → Filtrar → Analisar com IA → Salvar → Classificar → Visualizar
```

Novas funcionalidades e melhorias continuam sendo adicionadas.
