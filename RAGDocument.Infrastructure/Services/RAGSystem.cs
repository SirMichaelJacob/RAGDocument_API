using Microsoft.Extensions.Options;
using RAGDocument.Application.DTOs;
using RAGDocument.Application.Interfaces;
using RAGDocument.Application.Utilities;
using RAGDocument.Domain.Interfaces;

namespace RAGDocument.Infrastructure.Services
{
    public class RAGSystem: IRagService
    {
        private readonly EmbeddingService _embeddingService;
        private readonly LMStudioService _llmClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LlmSettings _settings;
        private int _topK;

        public RAGSystem(IUnitOfWork unitOfWork, EmbeddingService embeddingService, LMStudioService llmClient, IOptions<LlmSettings> settings)
        {
            _embeddingService = embeddingService;
            _unitOfWork = unitOfWork;
            _llmClient = llmClient;
            _settings = settings.Value;
            _topK = _settings.TopK;
        }


        public async Task<string> QueryAsync(string question, CancellationToken ct = default)
        {

            // Get embedding for the question
            float[] questionEmbedding = await _embeddingService.GenerateEmbeddingAsync(question);

            // Retrieve relevant documents
            List<string> relevantDocs = await SearchSimilarDocumentsAsync(questionEmbedding, _topK);

            if (relevantDocs.Count == 0)
            {
                return "I couldn't find any relevant information to answer your question.";
            }

            // Build context from retrieved documents
            string context = string.Join(
                "\n\n",
                relevantDocs.Select((doc, i) => $"Document {i + 1}: {doc}")
            );

            // Generate answer using LLM
            string prompt =
                $@"Based on the following information, please answer the question. Context:{context} Question: {question} Answer:";

            string answer = await _llmClient.GenerateAsync(prompt);
            return answer;
        }

        private async Task<List<string>> SearchSimilarDocumentsAsync(float[] queryEmbedding, int topK)
        {
            var vectors = await _unitOfWork.Document.GetDocumentEmbeddingsAsync();

            var scored = new List<(Guid id, double score)>();

            foreach (var v in vectors)
            {
                float[] docVector = Utility.ConvertBytesToFloatArray(v.Embedding);

                if (docVector.Length != queryEmbedding.Length)
                    continue;

                double similarity = Utility.CosineSimilarity(queryEmbedding, docVector);
                scored.Add((v.Id, similarity));
            }

            var topIds = scored
                .OrderByDescending(x => x.score)
                .Take(topK)
                .Select(x => x.id)
                .ToList();

            return await _unitOfWork.Document.GetContentsByIdsAsync(topIds);
        }


    }
}
