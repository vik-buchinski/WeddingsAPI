using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Models.Database.Common;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.Admin
{
    [RoutePrefix("api/admin/albums")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AdminAlbumsListController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAlbum()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType,
                    Properties.Resources.UnsupportedMediaTypeMessage);
            }
            string root = HttpContext.Current.Server.MapPath(Constants.IMG_UPLOADS_PATH);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                var token = provider.FormData.Get(Constants.SESSION_TOKEN_HEADER_KEY);
                if (String.IsNullOrEmpty(token))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }
                var session =
                    _dataRepositories.SessionModelRepository.FirstOrDefault(f => f.Token.Equals(token) && f.IsActive);
                if (null == session)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }

                var albumType = provider.FormData.Get("album_type");
                if (!Common.IsAlbumTypeExist(albumType))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.AlbumNotFound);
                }

                var albumName = provider.FormData.Get("album_name");
                if (String.IsNullOrEmpty(albumName))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, Properties.Resources.AlbumNameRequired);
                }
                var albumDescription = provider.FormData.Get("album_description");

                String uploadedFilePath = null;
                int? imageHeight = null;
                int? imageWidth = null;
                // This illustrates how to get the file names for uploaded files.
                foreach (MultipartFileData file in provider.FileData)
                {
                    if (null != file.Headers.ContentDisposition.Name &&
                        file.Headers.ContentDisposition.Name.Replace("\"", String.Empty)
                            .Equals("image"))
                    {
                        uploadedFilePath = file.LocalFileName;
                        FileStream fileStream = new FileStream(file.LocalFileName, FileMode.Open);
                        Image image = Image.FromStream(fileStream);
                        imageHeight = image.Height;
                        imageWidth = image.Width;
                        fileStream.Close();
                        fileStream.Dispose();
                        image.Dispose();
                    }
                    else
                    {
                        File.Delete(file.LocalFileName);
                    }
                }
                if (String.IsNullOrEmpty(uploadedFilePath))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent,
                        Properties.Resources.AlbumImageRequired);
                }

                var imageModel = new ImagesModel
                                 {
                                     LocalFileName = uploadedFilePath,
                                     Width = imageWidth,
                                     Height = imageHeight
                                 };
                _dataRepositories.ImagesModelRepository.Insert(imageModel);
                _dataRepositories.Save();
                //TODO: check if image id do not need request from database

                var newAlbum = new AlbumModel
                               {
                                   AlbumName = albumName,
                                   AlbumDescription = albumDescription,
                                   AlbumType = albumType,
                                   ImageId =
                                       _dataRepositories.ImagesModelRepository.FirstOrDefault(
                                           f => f.LocalFileName.Equals(uploadedFilePath)).Id
                               };
                _dataRepositories.AlbumModelRepository.Insert(newAlbum);
                _dataRepositories.Save();

                return Request.CreateResponse(HttpStatusCode.Created, "");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        
        [Route("{albumId}")]
        [HttpPut]
        public async Task<HttpResponseMessage> EditAlbum(int albumId)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType,
                    Properties.Resources.UnsupportedMediaTypeMessage);
            }
            string root = HttpContext.Current.Server.MapPath(Constants.IMG_UPLOADS_PATH);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                var token = provider.FormData.Get(Constants.SESSION_TOKEN_HEADER_KEY);
                if (String.IsNullOrEmpty(token))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }
                var session =
                    _dataRepositories.SessionModelRepository.FirstOrDefault(f => f.Token.Equals(token) && f.IsActive);
                if (null == session)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }

                var albumDescription = provider.FormData.Get("album_description");
                var albumName = provider.FormData.Get("album_name");
                var isAlbumVisible = provider.FormData.Get("is_visible");

                String uploadedFilePath = null;
                int? imageHeight = null;
                int? imageWidth = null;
                // This illustrates how to get the file names for uploaded files.
                foreach (MultipartFileData file in provider.FileData)
                {
                    if (null != file.Headers.ContentDisposition.Name &&
                        file.Headers.ContentDisposition.Name.Replace("\"", String.Empty)
                            .Equals("image"))
                    {
                        uploadedFilePath = file.LocalFileName;
                        FileStream fileStream = new FileStream(file.LocalFileName, FileMode.Open);
                        Image image = Image.FromStream(fileStream);
                        imageHeight = image.Height;
                        imageWidth = image.Width;
                        fileStream.Close();
                        fileStream.Dispose();
                        image.Dispose();
                    }
                    else
                    {
                        File.Delete(file.LocalFileName);
                    }
                }

                var updatingAlbum = _dataRepositories.AlbumModelRepository.GetById(albumId);

                if (null == updatingAlbum)
                {
                    if (!String.IsNullOrEmpty(uploadedFilePath))
                    {
                        File.Delete(uploadedFilePath);
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        Properties.Resources.AlbumNotFound);
                }

                if (null != uploadedFilePath)
                {
                    ImagesModel imageForUpdate;
                    if (null != updatingAlbum.ImageId)
                    {
                        imageForUpdate = _dataRepositories.ImagesModelRepository.GetById(updatingAlbum.ImageId);
                        if (null != imageForUpdate)
                        {
                            File.Delete(imageForUpdate.LocalFileName);
                            imageForUpdate.LocalFileName = uploadedFilePath;
                            imageForUpdate.Width = imageWidth;
                            imageForUpdate.Height = imageHeight;
                            _dataRepositories.ImagesModelRepository.Update(imageForUpdate);
                        }
                        else
                        {
                            imageForUpdate = new ImagesModel
                            {
                                Height = imageHeight,
                                Width = imageWidth,
                                LocalFileName = uploadedFilePath
                            };
                            _dataRepositories.ImagesModelRepository.Insert(imageForUpdate);
                            _dataRepositories.Save();
                            imageForUpdate =
                                _dataRepositories.ImagesModelRepository.FirstOrDefault(
                                    f => f.LocalFileName.Equals(uploadedFilePath));
                            updatingAlbum.ImageId = imageForUpdate.Id;
                        }
                    }
                    else
                    {
                        imageForUpdate = new ImagesModel
                        {
                            Height = imageHeight,
                            Width = imageWidth,
                            LocalFileName = uploadedFilePath
                        };
                        _dataRepositories.ImagesModelRepository.Insert(imageForUpdate);
                        _dataRepositories.Save();
                        imageForUpdate =
                            _dataRepositories.ImagesModelRepository.FirstOrDefault(
                                f => f.LocalFileName.Equals(uploadedFilePath));
                        updatingAlbum.ImageId = imageForUpdate.Id;
                    }
                }
                else if (!updatingAlbum.IsExpanded)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        Properties.Resources.AlbumImageRequired);
                }

                updatingAlbum.AlbumDescription = albumDescription;
                if (!updatingAlbum.IsExpanded)
                {
                    updatingAlbum.AlbumName = albumName;
                }
                if (!String.IsNullOrEmpty(isAlbumVisible))
                {
                    bool albumVisibility;
                    if (Boolean.TryParse(isAlbumVisible, out albumVisibility))
                    {
                        updatingAlbum.IsVisible = albumVisibility;
                    }
                }

                _dataRepositories.AlbumModelRepository.Update(updatingAlbum);
                _dataRepositories.Save();
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        
        [Route("{albumId}")]
        [HttpDelete]
        public HttpResponseMessage DeleteAlbum(int albumId)
        {
            var headers = Request.Headers;
            string token = null;
            if (headers.Contains(Constants.SESSION_TOKEN_HEADER_KEY))
            {
                token = headers.GetValues(Constants.SESSION_TOKEN_HEADER_KEY).First();
            }

            if (String.IsNullOrEmpty(token))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
            }
            var session =
                _dataRepositories.SessionModelRepository.FirstOrDefault(f => f.Token.Equals(token) && f.IsActive);
            if (null == session)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
            }

            var albumToDelete = _dataRepositories.AlbumModelRepository.GetById(albumId);

            if (null == albumToDelete)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    Properties.Resources.AlbumNotFound);
            }

            albumToDelete.IsVisible = false;
            _dataRepositories.AlbumModelRepository.Update(albumToDelete);
            _dataRepositories.Save();

            var imagesToDelete =
                _dataRepositories.ImagesModelRepository.Get(
                    f => f.AlbumId == albumToDelete.Id || f.Id == albumToDelete.ImageId);

            foreach (var imageToDeleteModel in imagesToDelete)
            {
                File.Delete(imageToDeleteModel.LocalFileName);
                _dataRepositories.ImagesModelRepository.Delete(imageToDeleteModel);
            }
            _dataRepositories.AlbumModelRepository.Delete(albumToDelete);
            _dataRepositories.Save();

            return Request.CreateResponse(HttpStatusCode.OK, "");
        }
        
        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}