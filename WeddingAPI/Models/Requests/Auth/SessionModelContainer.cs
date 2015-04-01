using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Auth
{
    public class SessionModelContainer
    {
        [JsonProperty(PropertyName = "session")]
        public SessionModel Session { get; set; }
    }
}