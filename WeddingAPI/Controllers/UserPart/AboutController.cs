using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Models.Requests.Admin.About;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.UserPart
{
    [RoutePrefix("api")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AboutController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("about")]
        [HttpGet]
        public HttpResponseMessage GetAbout()
        {
            var aboutModel = _dataRepositories.AdminAboutModelRepository.FirstOrDefault(f => true);

            ViewAdminAboutModel respModel = null;
            if (null != aboutModel)
            {
                respModel = new ViewAdminAboutModel
                    {
                        Description = aboutModel.Description
                    };
                if (null != aboutModel.ImageModelId)
                {
                    respModel.ImageUrl = Common.GenerateImageLink((int)aboutModel.ImageModelId,
                        Request.RequestUri.GetLeftPart(UriPartial.Authority));
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, respModel);
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}