using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Application.Interfaces
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
    }

    public interface ILMStudioService
    {
        Task<string> GenerateAsync(string prompt, int maxTokens = 500);
    }

    public interface IRagService
    {
        Task<string> QueryAsync(string question, CancellationToken ct = default);
    }
}
