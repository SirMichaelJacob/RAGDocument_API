using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RAGDocument.Application.DTOs;
using RAGDocument.Application.Interfaces;
using RAGDocument.Application.Utilities;
using RAGDocument.Domain.Interfaces;
using FaissNet;

namespace RAGDocument.Infrastructure.Services
{
    public class RAGSystem: IRagService
    {
        private readonly IMemoryCache _cache;
        private readonly EmbeddingService _embeddingService;
        private readonly LMStudioService _llmClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LlmSettings _settings;
        private int _topK;

        // FAISS index fields
        private FaissNet.Index _index;  // FAISS index
        private List<Guid> _idMap = new List<Guid>();  // Maps FAISS internal ID (sequential long) to document Guid

        public RAGSystem(IUnitOfWork unitOfWork, EmbeddingService embeddingService, LMStudioService llmClient, IOptions<LlmSettings> settings, IMemoryCache cache)
        {
            _embeddingService = embeddingService;
            _unitOfWork = unitOfWork;
            _llmClient = llmClient;
            _settings = settings.Value;
            _topK = _settings.TopK;
            _cache = cache;

            // Build FAISS index on startup
            InitializeIndexAsync().GetAwaiter().GetResult();  // Sync for simplicity; make async if needed
        }


        public async Task<string> QueryAsync(string question, CancellationToken ct = default)
        {
            int maxPromptTokens = _settings.MaxTokens; // 2048
            int reservedForAnswer = 500;
            int maxContextTokens = maxPromptTokens - reservedForAnswer;
            int maxContextWords = (int)(maxContextTokens / 1.3); // rough word count

            string cacheKey = $"answer:{question.ToLower().Trim()}";

            if (_cache.TryGetValue(cacheKey, out string cachedAnswer))
                return cachedAnswer;

            float[] questionEmbedding = await GetOrCacheEmbedding(question);

            List<string> relevantDocs = await GetOrCacheSearch(question, questionEmbedding);

            if (relevantDocs.Count == 0)
            {
                return "I couldn't find any relevant information to answer your question.";
            }
            int wordsPerDoc = maxContextWords / relevantDocs.Count;
            var truncatedDocs = relevantDocs
                .Select(d => Utility.TruncateText(d, wordsPerDoc))
                .ToList();

            string context = string.Join("\n\n", truncatedDocs.Select((doc, i) => $"Document {i + 1}: {doc}"));

            string prompt = $@"Based on the following information, please answer the question. 
                Context:{context} 
                Question: {question} 
                Answer:";

            string answer = await _llmClient.GenerateAsync(prompt);

            _cache.Set(cacheKey, answer, TimeSpan.FromMinutes(30));

            return answer;
        }

        private async Task<List<string>> SearchSimilarDocumentsAsync(float[] queryEmbedding, int topK)
        {
            if (_index == null || _idMap.Count == 0)
                return new List<string>();

            // Normalize query for cosine
            float[] normalizedQuery = Utility.Normalize(queryEmbedding);

            // Search: FAISS returns distances (similarities for IP) and IDs
            var (distances, ids) = _index.Search(new float[][] { normalizedQuery }, topK);

            // For METRIC_INNER_PRODUCT, higher distance = better similarity; already sorted descending
            var topInternalIds = ids[0].Take(topK);  // ids[0] for single query

            // Map back to Guids
            var topGuids = topInternalIds.Select(internalId => _idMap[(int)internalId]).ToList();  // Cast long to int (safe for <2^31 docs)

            return await _unitOfWork.Document.GetContentsByIdsAsync(topGuids);
        }
        private async Task<float[]> GetOrCacheEmbedding(string text)
        {
            string key = $"embed:{text.GetHashCode()}";

            if (_cache.TryGetValue(key, out float[] cached))
                return cached;

            var embedding = await _embeddingService.GenerateEmbeddingAsync(text);

            _cache.Set(key, embedding, TimeSpan.FromHours(6));

            return embedding;
        }

        private async Task<List<string>> GetOrCacheSearch(string question, float[] embedding)
        {
            string key = $"search:{question.GetHashCode()}";

            if (_cache.TryGetValue(key, out List<string> cached))
                return cached;

            var docs = await SearchSimilarDocumentsAsync(embedding, _settings.TopK);

            _cache.Set(key, docs, TimeSpan.FromMinutes(30));

            return docs;
        }

        private async Task InitializeIndexAsync()
        {
            var docEmbeddings = await _unitOfWork.Document.GetDocumentEmbeddingsAsync();
            if (!docEmbeddings.Any()) return;

            _idMap = docEmbeddings.Select(v => v.Id).ToList();

            // Normalize embeddings for cosine similarity
            var embeddingList = docEmbeddings
                .Select(v => Utility.Normalize(Utility.ConvertBytesToFloatArray(v.Embedding)))
                .ToArray();  // float[][] for FAISS

            int dimension = embeddingList[0].Length;  // Assume all same dim

            // Create FAISS index for cosine (inner product on normalized vectors)
            _index = FaissNet.Index.CreateDefault(dimension, MetricType.METRIC_INNER_PRODUCT);

            // Assign sequential IDs (0 to n-1 as long[])
            long[] ids = Enumerable.Range(0, embeddingList.Length).Select(i => (long)i).ToArray();

            // Add to index
            _index.AddWithIds(embeddingList, ids);
        }

        // Refresh index after new documents (rebuild for simplicity)
        public async Task RefreshIndexAsync()
        {
            _index?.Dispose();  // Clean up old
            _index = null;
            _idMap.Clear();
            await InitializeIndexAsync();
        }
    }
}
