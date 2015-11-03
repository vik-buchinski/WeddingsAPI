using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Models.Database.Common;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers
{

    [RoutePrefix("api")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class CommonController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("images/{id}")]
        [HttpGet]
        public HttpResponseMessage DownloadImage(int id)
        {
            ImagesModel image = _dataRepositories.ImagesModelRepository.GetById(id);
            if (null == image || !File.Exists(image.LocalFileName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.NotFountMessage);
            }

            HttpResponseMessage result = null;

            if (!File.Exists(image.LocalFileName))
            {
                return Request.CreateResponse(HttpStatusCode.Gone);
            }
            result = Request.CreateResponse(HttpStatusCode.OK);


            using (var fileStream = new FileStream(image.LocalFileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    result.Content = new StreamContent(fileStream);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue(string.Format("image/{0}", "jpg"));
                    return result;
                }
                catch (Exception e)
                {
                    TraceExceptionLogger.LogException(e);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}