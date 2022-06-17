using YourAPI.DAL.Repositories;
using YourAPI.DL.Context;
using YourAPI.DL.Models.DBModels;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace YourAPI.DAL
{
    public class UnitOfWork : IDisposable
    {
        private YourDbContext _context = new YourDbContext();

        //YourEntity
        private GenericRepository<YourEntity> _YourEntityRepository;
        public GenericRepository<YourEntity> YourEntityRepository
        {
            get
            {
                if (this._YourEntityRepository == null)
                {
                    this._YourEntityRepository = new GenericRepository<YourEntity>(_context);
                }
                return this._YourEntityRepository;
            }
        }

        public YourDbContext CurrentContext
        {
            get
            {
                return this._context;
            }
        }

        public IDbContextTransaction StartTransaction()
        {
            if (this._context != null)
            {
                return _context.Database.BeginTransaction();
            }
            else
            {
                return null;
            }
        }

        public async Task<IDbContextTransaction> StartTransactionAsync()
        {
            if (this._context != null)
            {
                return await _context.Database.BeginTransactionAsync();
            }
            else
            {
                return null;
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
