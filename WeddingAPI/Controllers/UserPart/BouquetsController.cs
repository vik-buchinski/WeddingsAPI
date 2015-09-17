using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.UserPart
{
    [RoutePrefix("api/bouquets")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class BouquetsController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetImages()
        {
            var album =
                _dataRepositories.AlbumModelRepository.FirstOrDefault(
                    f => f.AlbumType.Equals(Constants.AlbumTypes.BOUQUETS.ToString().ToLower()));
            if (null == album)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                Common.BuildRequestAlbumModel(_dataRepositories, Request.RequestUri.GetLeftPart(UriPartial.Authority),
                    album, false));
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}