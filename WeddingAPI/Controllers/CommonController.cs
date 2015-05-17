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

            FileStream fileStream = File.Open(image.LocalFileName, FileMode.Open);
            var response = new HttpResponseMessage { Content = new StreamContent(fileStream) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            response.Content.Headers.ContentLength = new FileInfo(image.LocalFileName).Length;
            return response;
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}