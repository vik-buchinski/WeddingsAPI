using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        
        [Route("")]
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
            var imageAlbum =
                _dataRepositories.AlbumModelRepository.FirstOrDefault(
                    f => f.AlbumType.Equals(Constants.AlbumTypes.BOUQUETS.ToString()));
            if (imageAlbum == null)
            {
                imageAlbum = new AlbumModel
                             {
                                 AlbumType = Constants.AlbumTypes.BOUQUETS.ToString(),
                                 IsExpanded = true
                             };
                _dataRepositories.AlbumModelRepository.Insert(imageAlbum);
                _dataRepositories.Save();
                imageAlbum =
                    _dataRepositories.AlbumModelRepository.FirstOrDefault(
                        f => f.AlbumType.Equals(Constants.AlbumTypes.BOUQUETS.ToString()));
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                Common.BuildRequestAlbumModel(_dataRepositories, Request.RequestUri.GetLeftPart(UriPartial.Authority),
                    imageAlbum, true));
        }
        
        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}