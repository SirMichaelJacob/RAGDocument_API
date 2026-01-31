using RAGDocument.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        void Delete(T entity);
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddRangeAsync(IQueryable<T> entities);
    }

    public interface IDocumentRepository : IRepository<Document>
    {
        Task<List<string>> GetContentAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByHashAsync(string hash,CancellationToken cancellationToken = default);
        Task<List<DocumentVectorDto>> GetDocumentEmbeddingsAsync();
        Task<List<string>> GetContentsByIdsAsync(List<Guid> ids);
    }
}
