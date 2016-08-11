using System;
using Daishi.NewRelic.Insights;
using Jil;

namespace WebService
{
    /// <summary>
    ///     <see cref="EventHubUploadFailedNewRelicInsightsEvent" /> is a New Relic
    ///     Insights event that encapsulates Event Hub failed-upload events
    /// </summary>
    internal class EventHubUploadFailedNewRelicInsightsEvent : NewRelicInsightsEvent
    {
        /// <summary>
        ///     <see cref="EventHubUploadFailedNewRelicInsightsEvent" /> public
        ///     constructor.
        /// </summary>
        public EventHubUploadFailedNewRelicInsightsEvent()
        {
            EventType = "ShieldInletErrors";
        }

        /// <summary>
        ///     <see cref="ErrorMessage" /> is the <see cref="Exception.Message" />.
        /// </summary>
        [JilDirective(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     <see cref="InnerErrorMessage" /> is the
        ///     <see cref="Exception.InnerException" />, if applicable.
        /// </summary>
        [JilDirective(Name = "innerErrorMessage")]
        public string InnerErrorMessage { get; set; }

        /// <summary>
        ///     <see cref="PartitionKey" /> is the Partition Key pertaining to the Event
        ///     Hub. Each stateless service instance contains a unique Partition Key, which
        ///     can be used to track specific instances that whereupon failures occur.
        /// </summary>
        [JilDirective(Name = "partitionKey")]
        public string PartitionKey { get; set; }

        /// <summary>
        ///     <see cref="UnixTimeStamp" /> is the current UTC-time expressed as Unix
        ///     ticks.
        /// </summary>
        [JilDirective(Name = "unixTimeStamp")]
        public int UnixTimeStamp
            => (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        /// <summary>
        ///     EventType is the New Relic Insights Event Grouping. It determines the
        ///     database to which the event will persist.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="NewRelicInsightsEvent.EventType" /><c>must</c> be serialised
        ///         in Camel case, in order to be correctly interpreted by New Relic
        ///         Insights.
        ///     </para>
        ///     <para>
        ///         Apply the following attribute to the
        ///         <see cref="NewRelicInsightsEvent.EventType" />
        ///         property in your implementation:
        ///     </para>
        ///     <para>
        ///         <c>[JilDirective(Name = "eventType")]</c>
        ///     </para>
        /// </remarks>
        [JilDirective(Name = "eventType")]
        public string EventType { get; set; }
    }
}