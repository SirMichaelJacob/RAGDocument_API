using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; }
        public byte[] Embedding { get; set; }
        public string ContentHash { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

    public class DocumentVectorDto
    {
        public Guid Id { get; set; }      // match your entity
        public byte[] Embedding { get; set; }
    }
}
