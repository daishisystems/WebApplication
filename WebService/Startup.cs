using System;
using System.Configuration;
using System.Web.Http;
using Daishi.NewRelic.Insights;
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

            // Start New Relic Insights Client

            NewRelicInsightsClient.Instance.NewRelicInsightsMetadata.AccountID =
                ConfigurationManager.AppSettings["NewRelicInsightsAccountID"];
            NewRelicInsightsClient.Instance.NewRelicInsightsMetadata.APIKey =
                ConfigurationManager.AppSettings["NewRelicInsightsAPIKey"];
            NewRelicInsightsClient.Instance.NewRelicInsightsMetadata.URI =
                new Uri("https://insights-collector.newrelic.com/v1/accounts");
            NewRelicInsightsClient.Instance.CacheUploadLimit = int.MaxValue;

            NewRelicInsightsClient.Instance.Initialise();

            // Connect to the Event Hub

            var eventHubConnectionString =
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var eventHubName =
                ConfigurationManager.AppSettings["EventHubName"];

            EventHubManager.Instance.Connect(eventHubConnectionString, eventHubName);
        }
    }
}