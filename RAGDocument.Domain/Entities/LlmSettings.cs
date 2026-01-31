using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Application.DTOs
{
    public class LlmSettings
    {
        public string EndPointUrl { get; set; } = "http://localhost:1234";
        public string LLMModel { get; set; } = "deepseek/deepseek-r1-0528-qwen3-8b";
        public string EmbeddingModel { get; set; } = "text-embedding-nomic-embed-text-v1.5";
        public int MaxRetries { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 600;
        public double Temperature { get; set; } = 0.5;
        public int TopK { get; set; } = 3;
        public int MaxTokens { get; set; }
    }
}
