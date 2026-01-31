using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Domain.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        IDocumentRepository Document { get; }

        Task SaveChangesAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default); // Wolverine uses this for outbox
    }
}
