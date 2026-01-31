using Microsoft.EntityFrameworkCore;
using RAGDocument.Domain.Entities;
using RAGDocument.Domain.Interfaces;
using RAGDocument.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Infrastructure.Repositories
{
    public class DocumentRepository:Repository<Document>,IDocumentRepository
    {
        public DocumentRepository(AppDbContext db) : base(db)
        {

        }

        public async Task<List<string>> GetContentAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.Select(x => x.Content).Where(x=>x!=null).AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByHashAsync(string hash,CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(x => x.ContentHash == hash, cancellationToken);

        }

        public async Task<List<DocumentVectorDto>> GetDocumentEmbeddingsAsync()
        {
            return await _dbSet.AsNoTracking().Select(d => new DocumentVectorDto
            {
                Id = d.Id,
                Embedding = d.Embedding
            })
            .ToListAsync();
        }

        public async Task<List<string>> GetContentsByIdsAsync(List<Guid> ids)
        {
            return await _dbSet.Where(d => ids.Contains(d.Id)).Select(d => d.Content).ToListAsync();
        }
    }
}
