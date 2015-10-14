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
    [RoutePrefix("api/admin/album")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AdminAlbumController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("{albumId}/image")]
        [HttpPost]
        public async Task<HttpResponseMessage> AddImage(int albumId)
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
                if (null == token || String.IsNullOrEmpty(token))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }
                var session =
                    _dataRepositories.SessionModelRepository.FirstOrDefault(f => f.Token.Equals(token) && f.IsActive);
                if (null == session)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }

                var description = provider.FormData.Get("description");

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

                if (!String.IsNullOrEmpty(uploadedFilePath))
                {
                    var imageModel = new ImagesModel
                    {
                        LocalFileName = uploadedFilePath,
                        Description = description,
                        AlbumId = updatingAlbum.Id,
                        Width = imageWidth,
                        Height = imageHeight
                    };
                    _dataRepositories.ImagesModelRepository.Insert(imageModel);
                    _dataRepositories.Save();
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, Properties.Resources.NoPhotoMessage);
                }

                return Request.CreateResponse(HttpStatusCode.Created, "");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("{albumId}")]
        [HttpPut]
        public async Task<HttpResponseMessage> EditImage(int albumId)
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
                if (null == token || String.IsNullOrEmpty(token))
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

                _dataRepositories.AlbumModelRepository.Update(updatingAlbum);
                _dataRepositories.Save();
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("{albumId}/image/{imageId}")]
        [HttpDelete]
        public HttpResponseMessage DeleteImage(int albumId, int imageId)
        {
            var headers = Request.Headers;
            string token = null;
            if (headers.Contains(Constants.SESSION_TOKEN_HEADER_KEY))
            {
                token = headers.GetValues(Constants.SESSION_TOKEN_HEADER_KEY).First();
            }

            if (null == token || String.IsNullOrEmpty(token))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
            }
            var session =
                _dataRepositories.SessionModelRepository.FirstOrDefault(f => f.Token.Equals(token) && f.IsActive);
            if (null == session)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
            }

            var updatingAlbum = _dataRepositories.AlbumModelRepository.GetById(albumId);

            if (null == updatingAlbum)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    Properties.Resources.AlbumNotFound);
            }

            var imageModel = _dataRepositories.ImagesModelRepository.GetById(imageId);

            if (null == imageModel || imageModel.AlbumId != updatingAlbum.Id)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.ImageNotFoundMessage);
            }

            File.Delete(imageModel.LocalFileName);
            _dataRepositories.ImagesModelRepository.Delete(imageModel);
            _dataRepositories.Save();

            return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        [Route("{albumId}/image/{imageId}")]
        [HttpPut]
        public async Task<HttpResponseMessage> EditImage(int albumId, int imageId)
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
                if (null == token || String.IsNullOrEmpty(token))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }
                var session =
                    _dataRepositories.SessionModelRepository.FirstOrDefault(f => f.Token.Equals(token) && f.IsActive);
                if (null == session)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
                }

                var description = provider.FormData.Get("description");
                //var imageIdForEdit = provider.FormData.Get("image_id");

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
                var imageModel = _dataRepositories.ImagesModelRepository.GetById(imageId);
                
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

                if (null == imageModel || imageModel.AlbumId != updatingAlbum.Id)
                {
                    if (!String.IsNullOrEmpty(uploadedFilePath))
                    {
                        File.Delete(uploadedFilePath);
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        Properties.Resources.ImageNotFoundMessage);
                }

                if (!String.IsNullOrEmpty(uploadedFilePath))
                {
                    File.Delete(imageModel.LocalFileName);
                    imageModel.LocalFileName = uploadedFilePath;
                    imageModel.Width = imageWidth;
                    imageModel.Height = imageHeight;
                }
                imageModel.Description = description;
                _dataRepositories.ImagesModelRepository.Update(imageModel);
                _dataRepositories.Save();
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}