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
    public class AlbumController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();
        /*
        [Route("album_images/{type}")]
        [HttpGet]
        public HttpResponseMessage GetImages(String type)
        {
            if (String.IsNullOrEmpty(type) || !Common.IsAlbumTypeExist(type))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.NotFountMessage);
            }
            var images = new List<RequestImageModel>();
            var albums = _dataRepositories.AlbumModelRepository.Get(f=> f.AlbumType == type);
            foreach (var albumModel in albums)
            {
                images.AddRange(Common.GetAlbumImages(_dataRepositories, Request.RequestUri.GetLeftPart(UriPartial.Authority), false,
                    albumModel.Id));
            }
            return Request.CreateResponse(HttpStatusCode.OK, Common.BuildRequestAlbumModel(_dataRepositories, Request.RequestUri.GetLeftPart(UriPartial.Authority),
                    imageAlbum, true));
        }*/

        [Route("album/{id}/images")]
        [HttpGet]
        public HttpResponseMessage GetImages(int albumId)
        {
            var album = _dataRepositories.AlbumModelRepository.Get(f => f.Id == albumId);
            if (null == album)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.NotFountMessage);
            }
            return Request.CreateResponse(HttpStatusCode.OK, Common.GetAlbumImages(_dataRepositories, Request.RequestUri.GetLeftPart(UriPartial.Authority), false,
                    albumId));
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}