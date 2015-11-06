using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Models.Database.Common;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.UserPart
{
    [RoutePrefix("api/albums")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AlbumsListController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();
        
        [Route("{albumType}")]
        [HttpGet]
        public HttpResponseMessage GetAlbumByType(string albumType)
        {
            if (!Common.IsAlbumTypeExist(albumType))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.AlbumNotFound);
            }

            IEnumerable<AlbumModel> albums =
                _dataRepositories.AlbumModelRepository.Get(f => f.AlbumType.ToLower().Equals(albumType.ToLower()) && f.IsVisible);

            return Request.CreateResponse(HttpStatusCode.OK,
                Common.BuildRequestAlbumsList(
                    _dataRepositories,
                    Request.RequestUri.GetLeftPart(UriPartial.Authority),
                    albums));
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}