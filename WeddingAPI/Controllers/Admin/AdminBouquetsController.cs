using System;
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
    [RoutePrefix("api/admin/bouquets")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AdminBouquetsController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("images")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadImage()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, Properties.Resources.UnsupportedMediaTypeMessage);
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
                // This illustrates how to get the file names for uploaded files.
                foreach (MultipartFileData file in provider.FileData)
                {
                    if (null != file.Headers.ContentDisposition.Name &&
                        file.Headers.ContentDisposition.Name.Replace("\"", String.Empty)
                              .Equals("image"))
                    {
                        uploadedFilePath = file.LocalFileName;
                    }
                    else
                    {
                        File.Delete(file.LocalFileName);
                    }
                }
                if (!String.IsNullOrEmpty(uploadedFilePath))
                {
                    var imageModel = new ImagesModel
                                     {
                                         LocalFileName = uploadedFilePath,
                                         Description = description,
                                         AlbumType = Constants.AlbumTypes.BOUQUETS.ToString()
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

        [Route("images/{imageId}")]
        [HttpPut]
        public async Task<HttpResponseMessage> EditImage(int imageId)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, Properties.Resources.UnsupportedMediaTypeMessage);
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
                // This illustrates how to get the file names for uploaded files.
                foreach (MultipartFileData file in provider.FileData)
                {
                    if (null != file.Headers.ContentDisposition.Name &&
                        file.Headers.ContentDisposition.Name.Replace("\"", String.Empty)
                              .Equals("image"))
                    {
                        uploadedFilePath = file.LocalFileName;
                    }
                    else
                    {
                        File.Delete(file.LocalFileName);
                    }
                }
                var imageModel = _dataRepositories.ImagesModelRepository.GetById(imageId);

                if (null == imageModel || !imageModel.AlbumType.Equals(Constants.AlbumTypes.BOUQUETS.ToString()))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.ImageNotFoundMessage);
                }

                if (!String.IsNullOrEmpty(uploadedFilePath))
                {
                    File.Delete(imageModel.LocalFileName);
                    imageModel.LocalFileName = uploadedFilePath;
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

        [Route("images")]
        [HttpGet]
        public HttpResponseMessage GetImages()
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
            return Request.CreateResponse(HttpStatusCode.OK, Common.GetBouquetImages(_dataRepositories, Request.RequestUri.GetLeftPart(UriPartial.Authority)));
        }

        [Route("images/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteImage(int id)
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

            var imageModel = _dataRepositories.ImagesModelRepository.GetById(id);

            if (null == imageModel || !imageModel.AlbumType.Equals(Constants.AlbumTypes.BOUQUETS.ToString()))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.ImageNotFoundMessage);
            }

            File.Delete(imageModel.LocalFileName);
            _dataRepositories.ImagesModelRepository.Delete(imageModel);
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