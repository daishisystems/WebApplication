using System.Web.Http;

namespace WebService.Controllers
{
    public class EventHubController : ApiController
    {
        public bool Get()
        {
            return EventHubManager.Instance.IsInitialised && EventHubManager.Instance.IsConnected;
        }
    }
}