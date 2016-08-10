using System.Configuration;
using System.Web.Http;
using Owin;

namespace WebService
{
    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
                );

            appBuilder.UseWebApi(config);

            // Connect to the Event Hub

            var eventHubConnectionString =
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var eventHubName =
                ConfigurationManager.AppSettings["EventHubName"];

            EventHubManager.Instance.Connect(eventHubConnectionString, eventHubName);
        }
    }
}