using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using WeddingAPI.DAL;
using WeddingAPI.Models.Requests.Auth;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.Admin
{
    [RoutePrefix("api/auth")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class AuthController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("login")]
        [HttpPost]
        public HttpResponseMessage CreateSession(JObject jsonData)
        {
            try
            {
                if (null != jsonData)
                {
                    var email = (string)jsonData["email"];
                    var password = (string)jsonData["password"];

                    if (null != email && null != password)
                    {
                        if (IsValidEmail(email))
                        {
                            var user =
                                _dataRepositories.UserModelRepository.FirstOrDefault(u => u.Email.Equals(email));
                            if (null != user)
                            {
                                if (PasswordHash.ValidatePassword(password,user.PasswordHash))
                                {
                                    var session =
                                        _dataRepositories.SessionModelRepository.FirstOrDefault(
                                            s => s.UserId == user.Id && s.IsActive);
                                    if (null == session)
                                    {
                                        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                                        session = new Models.Database.Auth.SessionModel { IsActive = true, Token = token, UserId = user.Id };
                                        _dataRepositories.SessionModelRepository.Insert(session);
                                        _dataRepositories.Save();
                                    }
                                    var sessionModel = new SessionModelContainer
                                        {
                                            Session = new SessionModel { Token = session.Token }
                                        };
                                    return Request.CreateResponse(HttpStatusCode.Created, sessionModel);
                                }
                            }
                        }
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Properties.Resources.InvalidAuthDataMessage);
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Properties.Resources.EmailOrPassMissingMessage);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [NonAction]
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [Route("users/{id}")]
        [HttpGet]
        public IHttpActionResult GetUser(int id)
        {
            var product = _dataRepositories.UserModelRepository.FirstOrDefault(model => model.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}