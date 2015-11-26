using System;
using WeddingAPI.Models.Database.Admin;
using WeddingAPI.Models.Database.Admin.About;
using WeddingAPI.Models.Database.Auth;
using WeddingAPI.Models.Database.Common;

namespace WeddingAPI.DAL
{
    public class Repositories : IDisposable
    {
        private readonly WeddingContext _context = new WeddingContext();

        private GenericRepository<UserModel> _userModelRepository;
        private GenericRepository<SessionModel> _sessionModelRepository;
        private GenericRepository<ImagesModel> _imagesModelRepository;
        private GenericRepository<AdminAboutModel> _adminAboutModelRepository;
        private GenericRepository<AlbumModel> _albumModelRepository;
        private GenericRepository<AdminContactsModel> _contactsModelRepository;
        private GenericRepository<TitleImageModel> _titleImageModelRepository;

        public GenericRepository<TitleImageModel> TitleImageModelRepository
        {
            get { return _titleImageModelRepository ?? (_titleImageModelRepository = new GenericRepository<TitleImageModel>(_context)); }
        }

        public GenericRepository<AdminContactsModel> ContactsModelRepository
        {
            get { return _contactsModelRepository ?? (_contactsModelRepository = new GenericRepository<AdminContactsModel>(_context)); }
        }

        public GenericRepository<AlbumModel> AlbumModelRepository
        {
            get { return _albumModelRepository ?? (_albumModelRepository = new GenericRepository<AlbumModel>(_context)); }
        }

        public GenericRepository<ImagesModel> ImagesModelRepository
        {
            get { return _imagesModelRepository ?? (_imagesModelRepository = new GenericRepository<ImagesModel>(_context)); }
        }

        public GenericRepository<AdminAboutModel> AdminAboutModelRepository
        {
            get { return _adminAboutModelRepository ?? (_adminAboutModelRepository = new GenericRepository<AdminAboutModel>(_context)); }
        }

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