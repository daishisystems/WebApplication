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
        public async Task<HttpResponseMessage> Post([FromBody] string events)
        {
            try
            {
                IEnumerable<ShieldEvent> shieldEvents;
                using (var shieldEventsReader = new StringReader(events))
                {
                    shieldEvents = JSON.Deserialize<IEnumerable<ShieldEvent>>(shieldEventsReader);
                }

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