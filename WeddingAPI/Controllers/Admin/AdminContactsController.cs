using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using WeddingAPI.DAL;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.Admin
{
    [RoutePrefix("api/admin/contacts")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AdminContactsController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();
        //TODO: create table for saving contacts edit info

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}