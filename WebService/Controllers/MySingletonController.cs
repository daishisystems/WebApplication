using System.Collections.Generic;
using System.Web.Http;

namespace WebService.Controllers
{
    public class MySingletonController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new[] {MySingleton.Instance.IsLoaded.ToString()};
        }
    }
}

// todo: Create a new Event Hub, publish to it, deploy. Calculate parallelism.