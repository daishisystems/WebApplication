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

            MySingleton.Instance.IsLoaded = true;
        }
    }

    public sealed class MySingleton
    {
        static MySingleton()
        {
        }

        private MySingleton()
        {
        }

        public static MySingleton Instance { get; } = new MySingleton();

        public bool IsLoaded { get; set; }
    }
}