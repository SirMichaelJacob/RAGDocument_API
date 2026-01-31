---

# 🧠 RAG Document API (.NET + LM Studio)

A **Retrieval-Augmented Generation (RAG)** API built with **ASP.NET Core**, **Entity Framework Core**, **Wolverine**, and **LM Studio**.

It allows you to:

* Store documents in SQL Server
* Convert documents into embeddings
* Perform semantic similarity search
* Generate contextual answers using a local LLM

---

## 🚀 Architecture Overview

```
Client → API → RAG Pipeline → LLM Response
```

### RAG Pipeline Steps

1. **Document Ingestion**

   * Text is stored in SQL Server
   * Embeddings are generated via LM Studio
   * Embeddings saved as vectors

2. **Query Flow**

   * Question → embedding
   * Cosine similarity search against stored documents
   * Top-K relevant documents selected
   * Context + Question sent to LLM
   * Answer returned to user

---

## 🏗 Tech Stack

| Layer       | Technology            |
| ----------- | --------------------- |
| API         | ASP.NET Core          |
| Messaging   | Wolverine             |
| ORM         | Entity Framework Core |
| Database    | SQL Server            |
| LLM Runtime | LM Studio             |
| Embeddings  | Nomic Embed Text v1.5 |
| Similarity  | Cosine Similarity     |

---

## 📁 Project Structure

```
RAGDocument.API            → Controllers / Startup
RAGDocument.Application    → Services / Commands / Handlers
RAGDocument.Domain         → Interfaces / Entities
RAGDocument.Infrastructure → EF Core / Repositories
```

---

## ⚙️ Configuration

Copy the example file and edit as needed:

```bash
cp appsettings.example.json appsettings.json
```

**Example `appsettings.example.json`:**

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
    "EmbeddingModel": "text-embedding-nomic-embed-text-v1.5",
    "MaxRetries": 3,
    "TimeoutSeconds": 600,
    "Temperature": 0.5,
    "MaxTokens": 2048,
    "TopK": 3
  },
  "AllowedHosts": "*"
}
```

### ⚠️ Notes

1. This example is **safe to share publicly** — no passwords or API keys.
2. For production, store secrets in **user secrets**, **environment variables**, or **Key Vault**.
3. Adjust `ConnectionStrings` and `LLMS` endpoint to match your local environment.

---

## 🛠 Getting Started

1. **Clone the repository**

```bash
git clone https://github.com/yourusername/RAGDocument_API.git
cd RAGDocument_API
```

2. **Install dependencies**

```bash
dotnet restore
```

3. **Apply database migrations**

```bash
dotnet ef database update
```

4. **Run the API**

```bash
dotnet run
```

Swagger UI available at:

```
https://localhost:{port}/swagger
```

---

## 📌 API Endpoints

### ➕ Add Document

```
POST /api/document/AddDocument
```

**Body (form-data)**

| Key     | Value         |
| ------- | ------------- |
| Content | Document text |

### ❓ Query Knowledge Base

```
POST /api/document/Query
```

**Body (form-data)**

| Key      | Value         |
| -------- | ------------- |
| Question | User question |

---

## 🧠 Similarity Search

The system uses **cosine similarity** to rank documents:

```csharp
similarity = dot(A, B) / (|A| * |B|)
```

Top-K documents are selected as context for the LLM.

---

## 🧩 Key Services

| Service          | Role                                |
| ---------------- | ----------------------------------- |
| EmbeddingService | Converts text to vector embeddings  |
| RAGSystem        | Orchestrates retrieval + LLM prompt |
| LMStudioService  | Generates LLM answers               |
| UnitOfWork       | Database access layer               |

---

## ⚠️ Current Limitations

* In-memory similarity search (not indexed)
* Not optimized for >50k documents
* No document chunking yet

---

## 🔮 Future Improvements

* SQL vector indexing (pgvector / SQL Server 2022 vector type)
* Document chunking for large documents
* Hybrid search (keyword + vector)
* Caching layer
* Streaming LLM responses

---

## 🛡 Security & Privacy

* **No secrets included** in example configuration
* Use `.gitignore` for any local `appsettings.Development.json`
* Production deployments should store sensitive keys separately

---
