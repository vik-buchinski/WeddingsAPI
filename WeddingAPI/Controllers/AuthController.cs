using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using WeddingAPI.DAL;
using WeddingAPI.Models.Auth;
using System.Linq;

namespace WeddingAPI.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("users")]
        [HttpGet]
        public IEnumerable<UserModel> GetAllUsers()
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            IEnumerable<UserModel> users
                = _dataRepositories.UserModelRepository.Get(orderBy: o => o.OrderByDescending(model => model.Id));
            return users;
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