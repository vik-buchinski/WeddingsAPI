using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using WeddingAPI.DAL;
using WeddingAPI.Models.Database.Admin;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.Admin
{
    [RoutePrefix("api/admin/contacts")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AdminContactsController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("description")]
        [HttpPut]
        public HttpResponseMessage EditDescription(JObject jsonData)
        {
            var headers = Request.Headers;
            string token = null;
            if (headers.Contains(Constants.SESSION_TOKEN_HEADER_KEY))
            {
                token = headers.GetValues(Constants.SESSION_TOKEN_HEADER_KEY).First();
            }

            if (String.IsNullOrEmpty(token))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
            }
            var session =
                _dataRepositories.SessionModelRepository.FirstOrDefault(f => f.Token.Equals(token) && f.IsActive);
            if (null == session)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Properties.Resources.BadTokenMessage);
            }

            if (null == jsonData)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Properties.Resources.RequiredDataMissing);
            }

            var description = (string)jsonData["description"];

            var descriptionMessage = _dataRepositories.ContactsModelRepository.FirstOrDefault(f => String.IsNullOrEmpty(f.DescriptionText) || !String.IsNullOrEmpty(f.DescriptionText));

            if (null == descriptionMessage)
            {
                descriptionMessage = new AdminContactsModel
                    {
                        DescriptionText = description
                    };
                _dataRepositories.ContactsModelRepository.Insert(descriptionMessage);
            }
            else
            {
                descriptionMessage.DescriptionText = description;
                _dataRepositories.ContactsModelRepository.Update(descriptionMessage);
            }
            _dataRepositories.Save();

            return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}