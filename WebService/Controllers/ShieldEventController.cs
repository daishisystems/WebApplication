using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Daishi.NewRelic.Insights;
using Jil;
using Microsoft.ServiceBus.Messaging;

namespace WebService.Controllers
{
    public class ShieldEventController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody] string postBody)
        {
            try
            {
                if (string.IsNullOrEmpty(postBody))
                {
                    throw new ArgumentNullException(nameof(postBody), "Empty POST Body.");
                }

                IEnumerable<ShieldEvent> shieldEvents;
                using (var shieldEventsReader = new StringReader(postBody))
                {
                    shieldEvents = JSON.Deserialize<IEnumerable<ShieldEvent>>(shieldEventsReader);
                }

                var numEventHubPartitions =
                    int.Parse(ConfigurationManager.AppSettings["NumEventHubPartitions"]);

                // 1. Loop through dictionary and extract single instances
                // 2. Pump Single instances using 'fake' partition (GUID)
                // 3. Need to split 32 Partitions among X stateless instances
                // 4. Pump multiple IPs as batch with Partition Key = IP Address

                // May need to split into groups -> single instance IP, multiple instance IP
                // Divide multiple instance IPs into groups - max 32

                var eventDataBatch = new List<EventData>();

                foreach (var shieldEvent in shieldEvents)
                {
                    StringWriter writer;

                    using (writer = new StringWriter())
                    {
                        JSON.Serialize(shieldEvent, writer);
                    }

                    var eventData = new EventData(Encoding.UTF8.GetBytes(writer.ToString()))
                    {
                        // todo: Split by IP, set Partition Key = IP.
                        // Multiple instances will all funnel each IP to its relevant partition!
                        // Even if there are delays in publishing to stream, from another instance,
                        // the last streaming unit will take into account previous SU events, and 
                        // capture the IPs.
                        PartitionKey = EventHubManager.Instance.PartitionKey
                    };

                    eventDataBatch.Add(eventData);
                }

                if (!EventHubManager.Instance.IsConnected)
                {
                    var eventHubConnectionString =
                        ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
                    var eventHubName =
                        ConfigurationManager.AppSettings["EventHubName"];

                    EventHubManager.Instance.Connect(eventHubConnectionString, eventHubName);
                }

                await EventHubManager.Instance.EventHubClient.SendBatchAsync(eventDataBatch);

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception exception)
            {
                if (!NewRelicInsightsClient.Instance.HasStarted)
                {
                    NewRelicInsightsClient.Instance.NewRelicInsightsMetadata.AccountID =
                        ConfigurationManager.AppSettings["NewRelicInsightsAccountID"];
                    NewRelicInsightsClient.Instance.NewRelicInsightsMetadata.APIKey =
                        ConfigurationManager.AppSettings["NewRelicInsightsAPIKey"];
                    NewRelicInsightsClient.Instance.NewRelicInsightsMetadata.URI =
                        new Uri("https://insights-collector.newrelic.com/v1/accounts");
                    NewRelicInsightsClient.Instance.CacheUploadLimit = int.MaxValue;

                    NewRelicInsightsClient.Instance.Initialise();
                }

                NewRelicInsightsClient.Instance.AddNewRelicInsightEvent(new EventHubUploadFailedNewRelicInsightsEvent
                {
                    ErrorMessage = exception.Message,
                    InnerErrorMessage = exception.InnerException?.Message ?? string.Empty,
                    PartitionKey = EventHubManager.Instance.PartitionKey
                });

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}