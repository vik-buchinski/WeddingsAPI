using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Models.Requests.Admin.About;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers
{
    [RoutePrefix("api/admin/about")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AboutController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("save")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostFile()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string root = HttpContext.Current.Server.MapPath(Constants.ABOUT_UPLOADS_PATH);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                var responseModel = new AdminAboutModel();
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                var description = provider.FormData.Get("description");

                if (!String.IsNullOrEmpty(description))
                {
                    responseModel.Description = description;
                }
                // This illustrates how to get the file names for uploaded files.
                foreach (MultipartFileData file in provider.FileData)
                {
                    if (null != file.Headers.ContentDisposition.Name &&
                        file.Headers.ContentDisposition.Name.Replace("\"", String.Empty)
                              .Equals("avatar_image"))
                    {
                        responseModel.ImageUrl = file.LocalFileName;
                    }
                    else
                    {
                        File.Delete(file.LocalFileName);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, responseModel);
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