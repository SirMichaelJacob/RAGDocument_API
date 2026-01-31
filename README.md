```markdown
# 🧠 RAG Document API

**A modern, local-first Retrieval-Augmented Generation (RAG) REST API** built with **.NET 8/9**, **ASP.NET Core**, **Entity Framework Core**, **Wolverine**, and **LM Studio**.

Store documents → generate embeddings → semantic search → contextual LLM answers — **100% local, no cloud dependency**.

## ✨ Features

- Upload and store documents in SQL Server
- Generate embeddings using Nomic Embed Text v1.5 (via LM Studio)
- Perform cosine-similarity-based semantic search
- Generate accurate, context-aware answers using a local LLM
- Clean layered architecture (Domain-Driven Design inspired)
- OpenAPI/Swagger documentation out of the box

## 🏗 Architecture Overview

```
Client ──→ ASP.NET Core API ──→ Wolverine Handlers ──→ RAG Pipeline ──→ LM Studio (LLM + Embeddings)
                                 └───────→ SQL Server (documents + embeddings)
```

### RAG Flow

1. **Ingestion**  
   Text → Embedding (Nomic) → Store document + vector in SQL Server

2. **Query**  
   Question → Embedding → Cosine similarity search → Top-K chunks  
   → Build prompt (context + question) → LLM → Answer

## 🛠 Tech Stack

| Layer              | Technology                        |
|--------------------|-----------------------------------|
| Web API            | ASP.NET Core                      |
| Command Handling   | Wolverine                         |
| ORM / Migrations   | Entity Framework Core             |
| Database           | Microsoft SQL Server              |
| LLM & Embeddings   | LM Studio (OpenAI-compatible API) |
| Embedding Model    | nomic-embed-text-v1.5             |
| Similarity         | Cosine similarity (in-memory)     |
| Documentation      | Swashbuckle / OpenAPI             |

## 📂 Project Structure

```
src/
├── RAGDocument.API                # ASP.NET Core Web API + Controllers
├── RAGDocument.Application        # Commands, Queries, Handlers, Services
├── RAGDocument.Domain             # Entities, Interfaces, Value Objects
└── RAGDocument.Infrastructure     # EF Core DbContext, Repositories, Persistence
```

## ⚙️ Configuration

1. Copy the example configuration file:

```bash
cp appsettings.example.json appsettings.json
# or on Windows:
copy appsettings.example.json appsettings.json
```

2. Review / edit `appsettings.json` (example):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "RAGDbConn": "Server=localhost;Database=RAGDocumentDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "LLMS": {
    "EndPointUrl": "http://localhost:1234",
    "LLMModel": "qwen2.5-14b-instruct",
    "EmbeddingModel": "nomic-embed-text-v1.5",
    "MaxRetries": 3,
    "TimeoutSeconds": 600,
    "Temperature": 0.7,
    "MaxTokens": 2048,
    "TopK": 4
  },
  "AllowedHosts": "*"
}
```

> **Security note**: Never commit `appsettings.json` or `appsettings.Development.json` containing real credentials. Use `.gitignore`, User Secrets, or Azure Key Vault in production.

## 🚀 Getting Started

### Prerequisites

- .NET 8 or .NET 9 SDK
- SQL Server (local or Docker)
- [LM Studio](https://lmstudio.ai/) running with:
  - LLM model loaded (e.g. Qwen 2.5 14B Instruct)
  - Embedding model: nomic-embed-text-v1.5
  - Local inference server enabled (default: http://localhost:1234)

### Steps

1. Clone the repository

```bash
git clone https://github.com/yourusername/RAGDocument_API.git
cd RAGDocument_API
```

2. Restore dependencies

```bash
dotnet restore
```

3. Apply database migrations

```bash
# Option A - from any project folder
dotnet ef database update --project src/RAGDocument.Infrastructure --startup-project src/RAGDocument.API

# Option B - if already in the API project folder
dotnet ef database update
```

4. Start LM Studio server (with both LLM and embedding model loaded)

5. Run the API

```bash
dotnet run --project src/RAGDocument.API
```

Swagger UI will be available at:  
👉 `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

## 📡 API Endpoints (Summary)

| Method | Endpoint                        | Description                     |
|--------|---------------------------------|---------------------------------|
| `POST` | `/api/document/AddDocument`     | Upload text document            |
| `POST` | `/api/document/Query`           | Ask question → get RAG answer   |

**Example – Add Document** (multipart/form-data)

```http
POST /api/document/AddDocument
Content-Type: multipart/form-data

Content: "The quick brown fox jumps over the lazy dog..."
```

**Example – Query**

```http
POST /api/document/Query
Content-Type: multipart/form-data

Question: What does the fox do?
```

## 🧩 Core Components

- **EmbeddingService** – generates vectors using LM Studio embedding endpoint
- **RAGSystem** – orchestrates retrieval + prompt building
- **LMStudioService** – calls chat completions endpoint
- **UnitOfWork / Repositories** – clean database access

## ⚠️ Current Limitations

- In-memory cosine similarity (not suitable for >50k documents)
- No automatic document chunking / splitting
- No hybrid (BM25 + vector) search yet
- No rate limiting or authentication (add in production)

## 🔮 Planned Improvements

- Vector indexing (SQL Server 2022 vector type or pgvector)
- Intelligent chunking & metadata filtering
- Hybrid retrieval (keyword + semantic)
- Response streaming
- Authentication & authorization
- Caching (Redis / in-memory)
- Evaluation metrics & reranking

## 📄 License

MIT License

Feel free to use, modify, and contribute!

## ❤️ Contributing

Pull requests, bug reports, and feature suggestions are welcome!

```md`.

Let me know if you'd like to add badges, GitHub Actions status, screenshots, a demo video link, or anything else!
