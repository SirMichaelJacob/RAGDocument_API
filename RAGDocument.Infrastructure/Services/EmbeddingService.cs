using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAGDocument.Application.DTOs;
using RAGDocument.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;


namespace RAGDocument.Infrastructure.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private int? _embeddingDimension = null;
        private readonly string _endPointUrl;
        private readonly LlmSettings _settings;


        public EmbeddingService(HttpClient httpClient, IOptions<LlmSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }


        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var request = new
            {
                input = text,
                model = _settings.EmbeddingModel
            };

            string jsonContent = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/v1/embeddings", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Embedding API failed: {response.StatusCode}");
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);

                var embeddingData = result?.data?[0]?.embedding;

                if (embeddingData == null)
                    throw new Exception("No embedding returned from API");

                float[] embedding = ((JArray)embeddingData).ToObject<float[]>();

                // Store the dimension size for reference
                if (_embeddingDimension == null)
                {
                    _embeddingDimension = embedding.Length;
                }

                return embedding;
            }
            catch (Exception ex)
            {
                
                throw; // Re-throw instead of returning dummy embedding
            }
        }

    }
}
