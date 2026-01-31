using Microsoft.EntityFrameworkCore;
using RAGDocument.Domain.Interfaces;
using RAGDocument.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RAGDocument.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public IDocumentRepository Document { get; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Document = new DocumentRepository(_db);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            await SaveChangesInternalAsync(ct);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            //await _db.SaveChangesAsync(ct);
            await SaveChangesInternalAsync(ct);
        }

        private async Task SaveChangesInternalAsync(CancellationToken ct = default)
        {
            var strategy = _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _db.Database.BeginTransactionAsync(ct);

                try
                {
                    await _db.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync(ct);

                    // You can inspect which entities were affected
                    var entries = ex.Entries;

                    foreach (var entry in entries)
                    {
                        //  Refresh entity values from the database
                        await entry.ReloadAsync(ct);//Optional
                    }

                    throw new DbUpdateConcurrencyException(
                        "A concurrency conflict occurred. The data may have been modified or deleted since it was loaded.",
                        ex
                    );
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(ct);
                    throw new Exception("An error occurred while saving changes.", ex);
                }
            });
        }

    }
}
