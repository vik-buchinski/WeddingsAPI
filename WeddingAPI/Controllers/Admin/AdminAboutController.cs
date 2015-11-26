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
using WeddingAPI.Models.Database.Admin.About;
using WeddingAPI.Models.Database.Common;
using WeddingAPI.Models.Requests.Admin.About;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.Admin
{
    [RoutePrefix("api/admin/about")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AdminAboutController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("save")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostFile()
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

                var description = provider.FormData.Get("description");

                String uploadedFilePath = null;
                String imageMimeType = null;
                // This illustrates how to get the file names for uploaded files.
                foreach (MultipartFileData file in provider.FileData)
                {
                    if (null != file.Headers.ContentDisposition.Name &&
                        file.Headers.ContentDisposition.Name.Replace("\"", String.Empty)
                              .Equals("content_image"))
                    {
                        uploadedFilePath = file.LocalFileName;
                        imageMimeType = file.Headers.ContentType.ToString();
                    }
                    else
                    {
                        File.Delete(file.LocalFileName);
                    }
                }
                int? imageId = null;
                if (!String.IsNullOrEmpty(uploadedFilePath))
                {
                    var imageModel = new ImagesModel
                        {
                            LocalFileName = uploadedFilePath,
                            MimeType = imageMimeType
                        };
                    _dataRepositories.ImagesModelRepository.Insert(imageModel);
                    _dataRepositories.Save();
                    imageId = _dataRepositories.ImagesModelRepository.FirstOrDefault(
                        f => f.LocalFileName.Equals(uploadedFilePath)).Id;
                }

                var aboutModel = _dataRepositories.AdminAboutModelRepository.FirstOrDefault(f => true);

                if (null == aboutModel)
                {
                    aboutModel = new AdminAboutModel
                        {
                            Description = description
                        };
                    if (null != imageId)
                    {
                        aboutModel.ImageModelId = imageId;
                    }
                    _dataRepositories.AdminAboutModelRepository.Insert(aboutModel);
                }
                else
                {
                    aboutModel.Description = description;
                    if (null != imageId)
                    {
                        if (null != aboutModel.ImageModelId)
                        {
                            var fileForDeletting =
                                _dataRepositories.ImagesModelRepository.GetById(aboutModel.ImageModelId);
                            if (null != fileForDeletting)
                            {
                                File.Delete(fileForDeletting.LocalFileName);
                                _dataRepositories.ImagesModelRepository.Delete(fileForDeletting);
                            }
                        }
                        aboutModel.ImageModelId = imageId;
                    }
                    _dataRepositories.AdminAboutModelRepository.Update(aboutModel);
                }
                _dataRepositories.Save();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                TraceExceptionLogger.LogException(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("get")]
        [HttpGet]
        public HttpResponseMessage GetAbout()
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

            var aboutModel = _dataRepositories.AdminAboutModelRepository.FirstOrDefault(f => true);

            var respModel = new ViewAdminAboutModel();
            if (null != aboutModel)
            {
                respModel.Description = aboutModel.Description;
                if (null != aboutModel.ImageModelId)
                {
                    respModel.ImageUrl = Common.GenerateImageLink(aboutModel.ImageModelId,
                        Request.RequestUri.GetLeftPart(UriPartial.Authority));
                }
            }
            var titleImageModel =
                _dataRepositories.TitleImageModelRepository.FirstOrDefault(
                    k => k.PageKey.ToLower().Equals(Constants.TitleImagesTypes.ABOUT.ToString().ToLower()));
            if (null != titleImageModel)
            {
                respModel.TitleImageUrl = Common.GenerateImageLink(titleImageModel.ImageId,
                    Request.RequestUri.GetLeftPart(UriPartial.Authority));
            }

            return Request.CreateResponse(HttpStatusCode.OK, respModel);
        }

        [Route("title_image")]
        [HttpDelete]
        public HttpResponseMessage DeleteImage()
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
            

            var titleImageModel =
                _dataRepositories.TitleImageModelRepository.FirstOrDefault(
                    k => k.PageKey.ToLower().Equals(Constants.TitleImagesTypes.ABOUT.ToString().ToLower()));
            if (null != titleImageModel)
            {
                var imageModel = _dataRepositories.ImagesModelRepository.GetById(titleImageModel.ImageId);
                File.Delete(imageModel.LocalFileName);
                _dataRepositories.TitleImageModelRepository.Delete(titleImageModel);
                _dataRepositories.ImagesModelRepository.Delete(imageModel);
                _dataRepositories.Save();
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NoContent, "");
            }

            return Request.CreateResponse(HttpStatusCode.OK, "");
        }


        [Route("title_image")]
        [HttpPost]
        public async Task<HttpResponseMessage> SaveTitleImage()
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

                String uploadedFilePath = null;
                String imageMimeType = null;
                // This illustrates how to get the file names for uploaded files.
                foreach (MultipartFileData file in provider.FileData)
                {
                    if (null != file.Headers.ContentDisposition.Name &&
                        file.Headers.ContentDisposition.Name.Replace("\"", String.Empty)
                              .Equals("image"))
                    {
                        uploadedFilePath = file.LocalFileName;
                        imageMimeType = file.Headers.ContentType.ToString();
                    }
                    else
                    {
                        File.Delete(file.LocalFileName);
                    }
                }

                if (String.IsNullOrEmpty(uploadedFilePath))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, Properties.Resources.NoPhotoMessage);
                }

                var titleImageModel =
                    _dataRepositories.TitleImageModelRepository.FirstOrDefault(
                        k => k.PageKey.ToLower().Equals(Constants.TitleImagesTypes.ABOUT.ToString().ToLower()));
                ImagesModel imageModel;
                if (null != titleImageModel)
                {
                    imageModel = _dataRepositories.ImagesModelRepository.GetById(titleImageModel.ImageId);
                    File.Delete(imageModel.LocalFileName);
                    imageModel.LocalFileName = uploadedFilePath;
                    imageModel.MimeType = imageMimeType;
                    _dataRepositories.ImagesModelRepository.Update(imageModel);
                    _dataRepositories.Save();
                }
                else
                {
                    imageModel = new ImagesModel { LocalFileName = uploadedFilePath, MimeType = imageMimeType };
                    _dataRepositories.ImagesModelRepository.Insert(imageModel);
                    _dataRepositories.Save();
                    imageModel =
                        _dataRepositories.ImagesModelRepository.FirstOrDefault(
                            f => f.LocalFileName.Equals(uploadedFilePath));
                    titleImageModel = new TitleImageModel
                        {
                            ImageId = imageModel.Id,
                            PageKey = Constants.TitleImagesTypes.ABOUT.ToString().ToLower()
                        };
                    _dataRepositories.TitleImageModelRepository.Insert(titleImageModel);
                    _dataRepositories.Save();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                TraceExceptionLogger.LogException(e);
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