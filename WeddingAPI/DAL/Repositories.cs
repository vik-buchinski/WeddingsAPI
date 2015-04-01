using System;
using WeddingAPI.Models.Database.Auth;

namespace WeddingAPI.DAL
{
    public class Repositories : IDisposable
    {
        private readonly WeddingContext _context = new WeddingContext();

        private GenericRepository<UserModel> _userModelRepository;
        private GenericRepository<SessionModel> _sessionModelRepository;


        public GenericRepository<SessionModel> SessionModelRepository
        {
            get { return _sessionModelRepository ?? (_sessionModelRepository = new GenericRepository<SessionModel>(_context)); }
        }

        public GenericRepository<UserModel> UserModelRepository
        {
            get { return _userModelRepository ?? (_userModelRepository = new GenericRepository<UserModel>(_context)); }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}