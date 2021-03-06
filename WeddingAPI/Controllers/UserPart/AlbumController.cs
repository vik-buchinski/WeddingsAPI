﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WeddingAPI.DAL;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.UserPart
{
    [RoutePrefix("api/album")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AlbumController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();
        
        [Route("{albumId}")]
        [HttpGet]
        public HttpResponseMessage GetAlbumById(int albumId)
        {
            var album = _dataRepositories.AlbumModelRepository.GetById(albumId);

            if (null == album || !album.IsVisible)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Properties.Resources.AlbumNotFound);
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