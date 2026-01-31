using RAGDocument.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Application.Commands
{
    public class CreateDocumentCommand
    {
        public string Content { get; set; }

    }

    public class QueryDocumentsCommand
    {
        public string Question { get; set; }
    }
}
