using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using WeddingAPI.DAL;
using WeddingAPI.Models.Requests.Common;
using WeddingAPI.Utils;

namespace WeddingAPI.Controllers.UserPart
{
    [RoutePrefix("api/contacts")]
    [EnableCors(origins: Constants.CLIENT_URL, headers: "*", methods: "*")]
    public class ContactsController : ApiController
    {
        private readonly Repositories _dataRepositories = new Repositories();

        [Route("message")]
        [HttpPost]
        public HttpResponseMessage SendMessage(JObject jsonData)
        {
            if (null == jsonData)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Properties.Resources.RequiredDataMissing);
            }

            var name = (string)jsonData["name"];
            if (String.IsNullOrEmpty(name))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Properties.Resources.RequiredNameMissing);
            }

            var email = (string)jsonData["email"];
            if (String.IsNullOrEmpty(email) && Common.IsValidEmail(email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Properties.Resources.RequiredEmailMissing);
            }

            var message = (string)jsonData["message"];
            if (String.IsNullOrEmpty(message))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Properties.Resources.RequiredMessageMissing);
            }
            var phone = (string)jsonData["phone"];


            var gmailClient = new System.Net.Mail.SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 25,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(/*"vik.buchinski@gmail.com", "4205Distum4205"*/"designforlifepl@gmail.com", "krakow2016")
            };
            var textMessage =
                "From: " + email + "<br/>";
            if (!string.IsNullOrEmpty(phone))
            {
                textMessage += "Phone: " + phone + "<br/>";
            }
            textMessage += "Name: " + name + "<br/>" +
                           "Message:<br/><br/>" + message;

            using (var msg = new System.Net.Mail.MailMessage("designforlifepl@gmail.com", "designforlifemg@gmail.com", "Client question", textMessage))
            {
                msg.IsBodyHtml = true;
                try
                {
                    gmailClient.Send(msg);
                    return Request.CreateResponse(HttpStatusCode.OK, "");
                }
                catch (Exception e)
                {
                    TraceExceptionLogger.LogException(e);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, Properties.Resources.ErrorSendMessage);
                }
            }
        }
        
        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetDescription()
        {
            var descriptionMessage = _dataRepositories.ContactsModelRepository.FirstOrDefault(f => String.IsNullOrEmpty(f.DescriptionText) || !String.IsNullOrEmpty(f.DescriptionText));
            var requestModel = new RequestContactsModel();

            if (null != descriptionMessage)
            {
                requestModel.Description = descriptionMessage.DescriptionText;
            }

            return Request.CreateResponse(HttpStatusCode.OK, requestModel);
        }

        protected override void Dispose(bool disposing)
        {
            _dataRepositories.Dispose();
            base.Dispose(disposing);
        }
    }
}