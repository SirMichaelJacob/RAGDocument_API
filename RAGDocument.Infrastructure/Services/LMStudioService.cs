using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RAGDocument.Application.DTOs;
using RAGDocument.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RAGDocument.Infrastructure.Services
{
    public class LMStudioService : ILMStudioService
    {
        private readonly HttpClient _httpClient;
        private int? _embeddingDimension = null;
        private readonly string _endPointUrl;
        private readonly LlmSettings _settings;

        public LMStudioService(HttpClient httpClient, IOptions<LlmSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GenerateAsync(string prompt, int maxTokens = 500)
        {
            // LM Studio's chat completions format
            var request = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = _settings.Temperature,
                max_tokens = _settings.MaxTokens,
                stream = false
            };

            string jsonContent = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                //Console.WriteLine("\n[DEBUG] Sending request to LM Studio...");
                //Console.WriteLine($"[DEBUG] Endpoint: /v1/chat/completions");
                //Console.WriteLine($"[DEBUG] Request: {jsonContent.Substring(0, Math.Min(200, jsonContent.Length))}...");

                var response = await _httpClient.PostAsync("/v1/chat/completions", content);

                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                   
                    return $"Error: API returned {response.StatusCode}. Response: {responseBody}";
                }

               

                dynamic result = JsonConvert.DeserializeObject(responseBody);

                // Extract the response
                if (result?.choices != null && result.choices.Count > 0)
                {
                    string answer = result.choices[0].message.content.ToString();
                    return answer;
                }

                return "No response generated from LM Studio.";
            }
            catch (HttpRequestException ex)
            {
                return $"Error communicating with LM Studio: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }

}
