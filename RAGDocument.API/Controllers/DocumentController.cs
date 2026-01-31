using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RAGDocument.Application.Commands;
using RAGDocument.Application.DTOs;
using RAGDocument.Application.Utilities;
using Wolverine;

namespace RAGDocument.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public DocumentController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpPost("AddDocument")]
        public async Task<IActionResult> UploadDocument([FromForm] CreateDocDto dto, CancellationToken ct = default)
        {
            var command = new CreateDocumentCommand { Content = dto.Content };
            var result = await _messageBus.InvokeAsync<CustomResult>(command, ct);
            return Ok(result);
        }

        [HttpPost("Query")]
        public async Task<IActionResult> QueryDocuments([FromForm] AskQuestionDto dto, CancellationToken ct = default)
        {
            var command = new QueryDocumentsCommand { Question = dto.Question };
            var result = await _messageBus.InvokeAsync<string>(command, ct);
            return Ok(result);
        }
    }
}
