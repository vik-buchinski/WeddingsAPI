using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.UserPart
{
    [RoutePrefix("api")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class BouquetsController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("bouquets")]
        [HttpGet]
        public HttpResponseMessage GetBouquets()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                Common.GetBouquetImages(_dataRepositories, Request.RequestUri.GetLeftPart(UriPartial.Authority)));
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}