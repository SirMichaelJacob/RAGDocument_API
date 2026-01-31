using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RAGDocument.Application.Commands;
using RAGDocument.Application.DTOs;
using RAGDocument.Application.Interfaces;
using RAGDocument.Infrastructure.Services;
using RAGDocument.Application.Utilities;
using RAGDocument.Domain.Entities;
using RAGDocument.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RAGDocument.Application.Handlers
{
    public class DocumentHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly EmbeddingService _embeddingService;
        private readonly IRagService _ragService;
        public DocumentHandler(IUnitOfWork unitOfWork, EmbeddingService embeddingService, IRagService ragService)
        {
            _unitOfWork = unitOfWork;
            _embeddingService = embeddingService;
            _ragService = ragService;
        }

        public async Task<CustomResult> Handle(CreateDocumentCommand command, CancellationToken cancellationToken = default)
        {
            var AlreadyExists = new CustomResult
            {
                IsSuccess = false,
                Message = "Document Already Exists",
                HttpResponse = new HttpResponseMessage(HttpStatusCode.Conflict)
            };

            var embedding = await _embeddingService.GenerateEmbeddingAsync(command.Content);


            Document document = new Document();
            document.Content   = command.Content;
            document.Embedding = Utility.ConvertFloatArrayToBytes(embedding);
            document.ContentHash = Utility.CreateContentHash(command.Content);

            //Check if document already exists
            if (await _unitOfWork.Document.ExistsByHashAsync(document.ContentHash))
                return AlreadyExists;

            //Add Embeddings and ContentHash logic
            await _unitOfWork.Document.AddAsync(document);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CustomResult
            {
                IsSuccess = true,
                Message = "Document added successfully.",
                HttpResponse = new HttpResponseMessage(HttpStatusCode.Created)
            };

        }

        public async Task<string> Handle(QueryDocumentsCommand command, CancellationToken cancellationToken = default)
        { 
            var question = command.Question;
            var result = await _ragService.QueryAsync(question);
            return result;
        }
    }
}
