using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Application.DTOs
{
    public class CreateDocDto
    {
        public string Content { get; set; }
    }

    public class AskQuestionDto
    {
        public string Question { get; set; }
    }

    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Embedding { get; set; }
        public string? Metadata { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    

}
