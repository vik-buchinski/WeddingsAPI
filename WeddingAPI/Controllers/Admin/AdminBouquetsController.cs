using System;
using System.Collections.Generic;
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
using WeddingAPI.Models.Requests.Admin.Bouquets;
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
                int imageId;
                if (!String.IsNullOrEmpty(uploadedFilePath))
                {
                    var imageModel = new ImagesModel
                                     {
                                         LocalFileName = uploadedFilePath,
                                         Description = description
                                     };
                    _dataRepositories.ImagesModelRepository.Insert(imageModel);
                    _dataRepositories.Save();
                    imageId = _dataRepositories.ImagesModelRepository.FirstOrDefault(
                        f => f.LocalFileName.Equals(uploadedFilePath)).Id;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, Properties.Resources.NoPhotoMessage);
                }

                var bouquetsAlbumModel =
                    _dataRepositories.AlbumModelRepository.FirstOrDefault(
                        f => f.Type.Equals(Constants.AlbumTypes.BOUQUETS.ToString()));

                if (bouquetsAlbumModel == null)
                {
                    bouquetsAlbumModel =
                        new AlbumModel
                        {
                            Type = Constants.AlbumTypes.BOUQUETS.ToString(),
                            Images = new List<int>()
                        };
                    _dataRepositories.AlbumModelRepository.Insert(bouquetsAlbumModel);
                    _dataRepositories.Save();
                    bouquetsAlbumModel =
                        _dataRepositories.AlbumModelRepository.FirstOrDefault(
                            f => f.Type.Equals(Constants.AlbumTypes.BOUQUETS.ToString()));
                }
                bouquetsAlbumModel.Images.Add(imageId);
                _dataRepositories.AlbumModelRepository.Update(bouquetsAlbumModel);
                _dataRepositories.Save();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("edit_image/{imageId}")]
        [HttpPost]
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

                if (null == imageModel)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.ImageNotFoundMessage);
                }

                if (!String.IsNullOrEmpty(uploadedFilePath))
                {
                    File.Delete(imageModel.LocalFileName);
                    imageModel.LocalFileName = uploadedFilePath;
                    imageModel.Description = description;
                    _dataRepositories.ImagesModelRepository.Update(imageModel);
                    _dataRepositories.Save();
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, Properties.Resources.NoPhotoMessage);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [NonAction]
        private List<BouquetImageModel> GetBouquetImages()
        {
            var bouquets = new List<BouquetImageModel>();
            var bouquetsAlbumModel =
                _dataRepositories.AlbumModelRepository.FirstOrDefault(
                    f => f.Type.Equals(Constants.AlbumTypes.BOUQUETS.ToString()));
            if (bouquetsAlbumModel == null)
            {
                return null;
            }
            foreach (var id in bouquetsAlbumModel.Images)
            {
                var image = _dataRepositories.ImagesModelRepository.GetById(id);
                bouquets.Add(new BouquetImageModel
                             {
                                 Id = image.Id,
                                 Description = image.Description,
                                 ImageUrl = Common.GenerateImageLink(image.Id,
                                     Request.RequestUri.GetLeftPart(UriPartial.Authority))
                             });
            }
            return bouquets;
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
            return Request.CreateResponse(HttpStatusCode.OK, GetBouquetImages());
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


            return Request.CreateResponse(HttpStatusCode.OK, GetBouquetImages());
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}