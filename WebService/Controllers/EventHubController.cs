using System.Net.Http;
using System.Web.Http;

namespace WebService.Controllers
{
    public class EventHubController : ApiController
    {
        public bool Get()
        {
            return EventHubManager.Instance.IsInitialised && EventHubManager.Instance.IsConnected;
        }

        public HttpResponseMessage Post([FromBody] string shieldEvents)
        {

            return null;
        }
    }

    internal class ShieldEvent
    {
        public string IPAddress { get; set; }
        public string OccurredAt { get; set; }
        public string PathAndQuery { get; set; }
    }
}